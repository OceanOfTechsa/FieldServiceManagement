using FieldServiceManagement.Business.AuditsBusiness;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Company;

using FieldServiceManagement.Business.Imports;
using FieldServiceManagement.Data.DataModels.DataImportHistory;
using FieldServiceManagement.ViewModels.DataImport;
using FieldServiceManagement.ViewModels.Extensions;   // SaDateTime()

namespace FieldServiceManagement.Business.CompanyBusiness
{
    public class CompanyBusiness
    {

        private static readonly string[] AllowedImportOptions = ["new", "update", "both"];
        private static readonly string[] AllowedCharsets = ["utf-8", "iso-8859-1", "windows-1252"];

        private static readonly string[] RequiredCompanyFields = ["name", "email", "phone"];
        private static readonly string[] RequiredAddressFields = ["company", "addressType", "street", "city", "zip"];
        private static readonly string[] RequiredNotesFields = ["parent", "content"];

        // A company cannot exist without a type and a Service and Billing address (the address ids are not nullable),
        // so these fields must be mapped whenever the import can create companies ("new" or "both")
        private static readonly string[] CreateRequiredCompanyFields =
            ["type", "serviceStreet", "serviceCity", "serviceZip", "billingStreet", "billingCity", "billingZip"];

        public async Task<Guid?> CreateCompanyAsync(CreateCompanyViewModel model, string email)
        {
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);
            model.OrganisationId = currentUser.OrganisationId;
            model.CreatedBy = currentUser.Id;
            model.UpdatedBy = currentUser.Id;

            var company = ObjectMapper.Mapper.Map<Company>(model);
            return await new CompanyRepository().CreateCompanyAsync(company, new LocalIPResolver().GetRoutedIPv4());
        }

        public async Task<CompanyDetailsViewModel> GetCompanyFullDetailsByIdAsync(Guid Id, string email)
        {
            var company = new CompanyRepository().GetById(Id);
            return await BuildCompanyDetailsViewModel(company!, email);
        }

        public async Task<IEnumerable<CompanyListItemViewModel>> GetCompaniesByUserEmailAsync(string Email)
        {
            var results = await new CompanyRepository().GetCompaniesByUserEmailAsync(Email);
            return ObjectMapper.Mapper.Map<List<CompanyListItemViewModel>>(results);
        }

        public async Task<CompanyViewModel?> GetCompanyByIdAsync(Guid Id)
        {
            var result = new CompanyRepository().GetById(Id);
            return ObjectMapper.Mapper.Map<CompanyViewModel?>(result);
        }

        public async Task<Guid?> UpdateCompanyAsync(CompanyViewModel model, string currentUserEmail)
        {
            var dbModel = ObjectMapper.Mapper.Map<Company>(model);
            dbModel.Id = model.Id;
            return await new CompanyRepository().UpdateCompanyAsync(dbModel, currentUserEmail, new LocalIPResolver().GetRoutedIPv4()!);
        }

        public async Task<BusinessResult> RemoveCompanyAddressLink(string CompanyId, Guid AddressId)
        {
            Guid compId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(CompanyId));
            await new EntityAddressRepository().RemoveCompanyAddressLink(compId, AddressId);
            return new BusinessResult { Success = true };
        }

        public async Task<BusinessResult> DeleteCompany(string CompanyId, string userEmail, string? ipAddress = null)
        {
            Guid compId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(CompanyId));

            var result = await new CompanyRepository().DeleteCompanyAsync(compId, userEmail, new LocalIPResolver().GetRoutedIPv4());

            if (!result.Success)
                return new BusinessResult { Success = false, Message = result.ErrorMessage };

            return new BusinessResult { Success = true };
        }

        public async Task<ImportStartResult> StartCompanyImportAsync(CompanyImportRequest request, string email)
        {
            static ImportStartResult Fail(string message) => new() { Success = false, Message = message };

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);
            var dataImport = new DataImportBusiness();

            // 1. one import at a time per organisation
            var blocked = await dataImport.CheckCanStartAsync(currentUser.OrganisationId);
            if (blocked is not null)
                return Fail(blocked);

            var (maxFileBytes, maxRecords) = await dataImport.GetImportLimitsAsync(currentUser.OrganisationId);

            // 2. options
            if (request.CompaniesFile is null && request.AddressesFile is null && request.NotesFile is null)
                return Fail("Upload at least one file.");

            // the import options only apply when a Companies file is uploaded
            var option = request.CompaniesFile is null ? "new" : (request.ImportOption ?? "new").ToLowerInvariant();
            if (!AllowedImportOptions.Contains(option))
                return Fail("Unknown import option.");

            if (request.CompaniesFile is not null)
            {
                var by = (option == "new" ? request.SkipExistingBasedOn : request.FindExistingBasedOn)?.ToLowerInvariant();

                if (option != "new" && string.IsNullOrWhiteSpace(by))
                    return Fail("Choose how to find existing companies.");
                if (by is not null && by is not ("id" or "name" or "none"))
                    return Fail("Unknown way to find existing companies.");
            }

            foreach (var (cs, label) in new[] { (request.CompaniesCharset, "Companies"),(request.AddressesCharset, "Addresses"),(request.NotesCharset, "Notes") })
            {
                if (cs is not null && !AllowedCharsets.Contains(cs.ToLowerInvariant()))
                    return Fail($"{label}: unsupported character set.");
            }

            // 3. files and record counts
            var bytes = new Dictionary<string, byte[]>();
            var files = new Dictionary<string, object>();
            var totalRecords = 0;

            foreach (var (file, label, charset) in new[]
            {
                    (request.CompaniesFile, "Companies", (request.CompaniesCharset ?? "utf-8").ToLowerInvariant()),
                    (request.AddressesFile, "Addresses", (request.AddressesCharset ?? "iso-8859-1").ToLowerInvariant()),
                    (request.NotesFile, "Notes", (request.NotesCharset ?? "iso-8859-1").ToLowerInvariant())
            })
            {
                if (file is null) continue;

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return Fail($"{label}: only .csv files are allowed.");
                if (file.Length == 0)
                    return Fail($"{label}: the file is empty.");
                if (file.Length > maxFileBytes)
                    return Fail($"{label}: the file must not be larger than {maxFileBytes / (1024 * 1024)} MB.");

                var data = await ImportFiles.ReadAllBytesAsync(file);
                var records = ImportCsv.Parse(data, charset).Count - 1;   // minus the header row

                if (records <= 0)
                    return Fail($"{label}: the file has no data rows.");
                if (records > maxRecords)
                    return Fail($"{label}: the file has {records:N0} records, but your plan allows up to {maxRecords:N0} per file.");

                bytes[label] = data;
                totalRecords += records;
                files[label] = new { FileName = file.FileName, Module = label, Records = records };
            }

            // 4. mappings
            var companiesMapping = request.CompaniesFile is null ? null : ImportFiles.ParseMapping(request.CompaniesMapping);
            if (request.CompaniesFile is not null)
            {
                if (companiesMapping is null)
                    return Fail("The companies mapping is invalid.");

                var missing = RequiredCompanyFields.Where(f => !companiesMapping.ContainsKey(f)).ToList();
                if (missing.Count > 0)
                    return Fail($"Companies: map the mandatory fields: {string.Join(", ", missing)}.");

                if (option != "update")
                {
                    var missingForNew = CreateRequiredCompanyFields.Where(f => !companiesMapping.ContainsKey(f)).ToList();
                    if (missingForNew.Count > 0)
                        return Fail($"Companies: a new company needs a type and both a Service and a Billing address. Map: {string.Join(", ", missingForNew)}.");
                }

                var matchBy = option == "new" ? request.SkipExistingBasedOn : request.FindExistingBasedOn;
                if (string.Equals(matchBy, "id", StringComparison.OrdinalIgnoreCase) && !companiesMapping.ContainsKey("id"))
                    return Fail("Companies: map the Company Id field, which is used to find existing companies.");
            }

            var addressesMapping = request.AddressesFile is null ? null : ImportFiles.ParseMapping(request.AddressesMapping);
            if (request.AddressesFile is not null)
            {
                if (addressesMapping is null)
                    return Fail("The addresses mapping is invalid.");

                var missingAddr = RequiredAddressFields.Where(f => !addressesMapping.ContainsKey(f)).ToList();
                if (missingAddr.Count > 0)
                    return Fail($"Addresses: map the mandatory fields: {string.Join(", ", missingAddr)}.");
            }

            var notesMapping = request.NotesFile is null ? null : ImportFiles.ParseMapping(request.NotesMapping);
            if (request.NotesFile is not null)
            {
                if (notesMapping is null)
                    return Fail("The notes mapping is invalid.");

                var missingNotes = RequiredNotesFields.Where(f => !notesMapping.ContainsKey(f)).ToList();
                if (missingNotes.Count > 0)
                    return Fail($"Notes: map the mandatory fields: {string.Join(", ", missingNotes)}.");
            }

            // 5. build the job
            var job = new CompanyImportJob
            {
                OrganisationId = currentUser.OrganisationId,
                UserId = currentUser.Id,
                UserEmail = email,
                InitiatedAt = DateTime.Now.SaDateTime(),

                ImportOption = option,
                SkipExistingBasedOn = request.SkipExistingBasedOn,
                FindExistingBasedOn = request.FindExistingBasedOn,
                DontUpdateEmpty = request.DontUpdateEmpty,

                CompaniesCsv = bytes.GetValueOrDefault("Companies"),
                AddressesCsv = bytes.GetValueOrDefault("Addresses"),
                NotesCsv = bytes.GetValueOrDefault("Notes"),

                CompaniesMapping = companiesMapping,
                AddressesMapping = addressesMapping,
                NotesMapping = notesMapping,

                CompaniesCharset = (request.CompaniesCharset ?? "utf-8").ToLowerInvariant(),
                AddressesCharset = (request.AddressesCharset ?? "iso-8859-1").ToLowerInvariant(),
                NotesCharset = (request.NotesCharset ?? "iso-8859-1").ToLowerInvariant(),

                CompaniesFileName = request.CompaniesFile?.FileName ?? "",
                AddressesFileName = request.AddressesFile?.FileName ?? "",
                NotesFileName = request.NotesFile?.FileName ?? ""
            };

            // 6. save the history row. The database allows only one active import per organisation, even for simultaneous requests.
            var addError = await dataImport.AddAsync(new DataImportHistory
            {
                Id = job.ImportId,
                OrganisationId = job.OrganisationId,
                UserId = job.UserId,
                UserEmail = email,
                Module = "Companies",
                StatusId = DataImportStatus.Queued,
                ImportOption = option,
                FilesJson = System.Text.Json.JsonSerializer.Serialize(files),
                TotalRecords = totalRecords,
                CreatedAt = job.InitiatedAt
            });

            if (addError is not null)
                return Fail(addError);

            // 7. queue it. The worker picks it up in Step 3.
            await ImportQueue.CompanyJobs.Writer.WriteAsync(job);

            return new ImportStartResult { Success = true, ImportId = job.ImportId };
        }

        #region PRIVATE METHODS
        private static async Task<CompanyDetailsViewModel> BuildCompanyDetailsViewModel(Company company, string currentUserEmail)
        {
            var model = new CompanyDetailsViewModel();
            if (company is null)
                return model;

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);
            if (currentUser is null || company.OrganisationId != currentUser.OrganisationId)
                return model;

            model.Company = ObjectMapper.Mapper.Map<CompanyViewModel>(company);

            var orgId = currentUser.OrganisationId;

            var auditLogsTask = new AuditLogBusiness().GetByEntityAsync("Company", company.Id, orgId);
            var createdByTask = new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(company.CreatedBy);
            var linkedAddressesTask = new AddressBusiness.AddressBusiness().GetEntityLinkedAddressesAsync(EntityTypes.Company, company.Id, orgId);
            var contactsTask = new ContactBusiness.ContactBusiness().GetContactsByCompanyId(company.Id, orgId);

            await Task.WhenAll(auditLogsTask, createdByTask, linkedAddressesTask, contactsTask);

            model.AuditLogs = auditLogsTask.Result;
            model.LinkedAddresses = linkedAddressesTask.Result;
            model.Contacts = contactsTask.Result;

            var createdBy = createdByTask.Result;
            model.CreatedBy = createdBy;

            if (company.UpdatedBy.HasValue)
            {
                model.UpdatedBy = company.UpdatedBy.Value == createdBy.Id
                    ? createdBy
                    : await new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(company.UpdatedBy.Value);
            }

            return model;
        }

        #endregion
    }
}