using System.Text;
using Microsoft.AspNetCore.Http;
using FieldServiceManagement.Enum;
using Microsoft.EntityFrameworkCore;
using FieldServiceManagement.ViewModels.User;
using FieldServiceManagement.Business.Imports;
using FieldServiceManagement.ViewModels.Contact;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Business.AuditsBusiness;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Data.DataModels.DataImportHistory;
using FieldServiceManagement.Data.Extensions;


namespace FieldServiceManagement.Business.ContactBusiness
{
    public class ContactBusiness
    {
        private static readonly string[] AllowedImportOptions = ["new", "update", "both"];
        private static readonly string[] RequiredContactFields = ["name", "surname", "email", "company"];
        private static readonly string[] RequiredAddressFields = ["contact", "addressType", "street", "city", "zip"];
        private static readonly string[] RequiredNotesFields = ["parent", "content"];
        private static readonly string[] AllowedCharsets = ["utf-8", "iso-8859-1", "windows-1252"];

        public async Task<Guid?> CreateContactAsync(CreateContactViewModel model, string email)
        {
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);
            model.OrganisationId = currentUser.OrganisationId;
            model.CreatedBy = currentUser.Id;
            model.UpdatedBy = currentUser.Id;

            var dbModel = ObjectMapper.Mapper.Map<Contact>(model);
            return await new ContactRepository().CreateContactAsync(dbModel, new LocalIPResolver().GetRoutedIPv4());
        }

        public async Task<ContactDetailsViewModel?> GetContactFullDetailsByIdAsync(Guid id, string email)
        {
            var contact = new ContactRepository().GetById(id);
            return await BuildContactDetailsViewModel(contact!, email);
        }

        public async Task<IEnumerable<ContactListItemViewModel>> GetContactsByUserEmailAsync(string email)
        {
            var results = await new ContactRepository().GetContactsByUserEmailAsync(email);
            return ObjectMapper.Mapper.Map<List<ContactListItemViewModel>>(results);
        }

        public async Task<List<ContactViewModel>> GetContactsByCompanyId(Guid companyId, Guid organisationId)
        {
            var dataModel = await new ContactRepository().GetContactsByCompanyId(companyId, organisationId);
            return ObjectMapper.Mapper.Map<List<ContactViewModel>>(dataModel);
        }

        public async Task<ContactViewModel> GetContactByIdAsync(Guid id)
        {
            var results = new ContactRepository().GetById(id);
            return ObjectMapper.Mapper.Map<ContactViewModel>(results);
        }

        public async Task<BusinessResult> DeleteContact(string contactId, string userEmail, string? ipAddress = null)
        {
            Guid contactGuid;
            try
            {
                contactGuid = Guid.Parse(UrlEncryptionBusiness.DecryptParam(contactId));
            }
            catch
            { 
                return new BusinessResult { Success = false, Message = "Invalid reference." };
            }

            var result = await new ContactRepository().DeleteContactAsync(contactGuid, userEmail, ipAddress ?? new LocalIPResolver().GetRoutedIPv4());

            if (!result.Success)
            { 
                return new BusinessResult { Success = false, Message = result.ErrorMessage };
            }
            
            return new BusinessResult { Success = true };
        }
        
        public async Task<BusinessResult> UpdateContactAsync(ContactViewModel model, string currentUserEmail)
        {
            var dbModel = ObjectMapper.Mapper.Map<Contact>(model);
            dbModel.Id = model.Id;
 
            var result = await new ContactRepository()
                .UpdateContactAsync(dbModel, currentUserEmail, new LocalIPResolver().GetRoutedIPv4());
 
            if (!result.Success)
            {
                 return new BusinessResult { Success = false, Message = result.ErrorMessage };
            }
 
            return new BusinessResult { Success = true };
        }


        public async Task<ContactImportStartResult> StartContactImportAsync(ContactImportRequest request, string email)
        {

            if (request.ContactsFile is null &&
                request.AddressesFile is null &&
                request.NotesFile is null)
                return Fail("Upload at least one file.");

            // 1. Options
            var option = request.ContactsFile is null
                ? "new"
                : (request.ImportOption ?? "new").ToLowerInvariant();

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);

            var blocked = await new DataImportBusiness()
                .CheckCanStartAsync(currentUser.OrganisationId);

            if (blocked is not null)
                return Fail(blocked);

            var (MaxFileBytes, MaxRecords) = await GetContactImportLimitsAsync(currentUser);

            if (!AllowedImportOptions.Contains(option))
                return Fail("Unknown import option.");

            if (option != "new" && string.IsNullOrWhiteSpace(request.FindExistingBasedOn))
                return Fail("Choose how to find existing contacts.");

            foreach (var (cs, label) in new[]
            {
                    (request.ContactsCharset, "Contacts"),
                    (request.AddressesCharset, "Addresses"),
                    (request.NotesCharset, "Notes")
                })
            {
                if (cs is not null && !AllowedCharsets.Contains(cs.ToLowerInvariant()))
                    return Fail($"{label}: unsupported character set.");
            }

            // 2. Files and record counts
            var totalRecords = 0;
            var files = new Dictionary<string, object>();

            foreach (var (file, label, charset) in new[]
            {
                    (request.ContactsFile, "Contacts", request.ContactsCharset ?? "utf-8"),
                    (request.AddressesFile, "Addresses", request.AddressesCharset ?? "iso-8859-1"),
                    (request.NotesFile, "Notes", request.NotesCharset ?? "iso-8859-1")
                })
            {
                if (file is null) continue;

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    return Fail($"{label}: only .csv files are allowed.");

                if (file.Length == 0)
                    return Fail($"{label}: the file is empty.");

                if (file.Length > MaxFileBytes)
                    return Fail($"{label}: the file must not be larger than {MaxFileBytes / (1024 * 1024)} MB.");

                var records = await CountCsvRecordsAsync(file, charset);

                if (records == 0)
                    return Fail($"{label}: the file has no data rows.");

                if (records > MaxRecords)
                    return Fail($"{label}: the file has {records:N0} records, but your plan allows up to {MaxRecords:N0} per file.");

                totalRecords += records;

                files[label] = new
                {
                    FileName = file.FileName,
                    Module = label,
                    Records = records
                };
            }

            // 3. Mappings
            var contactsMapping = request.ContactsFile is null
                ? null
                : ParseMapping(request.ContactsMapping);

            if (request.ContactsFile is not null)
            {
                if (contactsMapping is null)
                    return Fail("The contacts mapping is invalid.");

                var missing = RequiredContactFields
                    .Where(f => !contactsMapping.ContainsKey(f))
                    .ToList();

                if (missing.Count > 0)
                    return Fail($"Contacts: map the mandatory fields: {string.Join(", ", missing)}.");

                var matchBy = option == "new"
                    ? request.SkipExistingBasedOn
                    : request.FindExistingBasedOn;

                if (string.Equals(matchBy, "id", StringComparison.OrdinalIgnoreCase) &&
                    !contactsMapping.ContainsKey("id"))
                    return Fail("Contacts: map the Contact Id field, which is used to find existing contacts.");
            }

            var addressesMapping = request.AddressesFile is null
                ? null
                : ParseMapping(request.AddressesMapping);

            if (request.AddressesFile is not null && addressesMapping is null)
                return Fail("The addresses mapping is invalid.");

            var notesMapping = request.NotesFile is null
                ? null
                : ParseMapping(request.NotesMapping);

            if (request.NotesFile is not null && notesMapping is null)
                return Fail("The notes mapping is invalid.");

            if (addressesMapping is not null)
            {
                var missingAddr = RequiredAddressFields
                    .Where(f => !addressesMapping.ContainsKey(f))
                    .ToList();

                if (missingAddr.Count > 0)
                    return Fail($"Addresses: map the mandatory fields: {string.Join(", ", missingAddr)}.");
            }

            if (notesMapping is not null)
            {
                var missingNotes = RequiredNotesFields
                    .Where(f => !notesMapping.ContainsKey(f))
                    .ToList();

                if (missingNotes.Count > 0)
                    return Fail($"Notes: map the mandatory fields: {string.Join(", ", missingNotes)}.");
            }

            // 4. Build the job
            var job = new ContactImportJob
            {
                OrganisationId = currentUser.OrganisationId,
                UserId = currentUser.Id,
                UserEmail = email,
                ImportOption = option,
                SkipExistingBasedOn = request.SkipExistingBasedOn,
                FindExistingBasedOn = request.FindExistingBasedOn,
                DontUpdateEmpty = request.DontUpdateEmpty,

                ContactsFileName = request.ContactsFile?.FileName ?? "",
                AddressesFileName = request.AddressesFile?.FileName ?? "",
                NotesFileName = request.NotesFile?.FileName ?? "",
                InitiatedAt = DateTime.Now.SaDateTime(),


                ContactsCsv = request.ContactsFile is null
                    ? null
                    : await ReadAllBytesAsync(request.ContactsFile),

                ContactsMapping = contactsMapping,

                AddressesCsv = request.AddressesFile is null
                    ? null
                    : await ReadAllBytesAsync(request.AddressesFile),

                AddressesMapping = addressesMapping,

                NotesCsv = request.NotesFile is null
                    ? null
                    : await ReadAllBytesAsync(request.NotesFile),

                NotesMapping = notesMapping,

                ContactsCharset = request.ContactsCharset ?? "utf-8",
                AddressesCharset = request.AddressesCharset ?? "iso-8859-1",
                NotesCharset = request.NotesCharset ?? "iso-8859-1"
            };

            // 5. Save import history
            var addError = await new DataImportBusiness().AddAsync(new DataImportHistory
            {
                Id = job.ImportId,
                OrganisationId = job.OrganisationId,
                UserId = job.UserId,
                UserEmail = email,
                Module = "Contacts",
                StatusId = DataImportStatus.Queued,
                ImportOption = option,
                FilesJson = System.Text.Json.JsonSerializer.Serialize(files),
                TotalRecords = totalRecords,
                CreatedAt = job.InitiatedAt
            });



            if (addError is not null)
                return Fail(addError);

            // 6. Queue the import
            await ImportQueue.Jobs.Writer.WriteAsync(job);

            return new ContactImportStartResult
            {
                Success = true,
                ImportId = job.ImportId
            };

            static ContactImportStartResult Fail(string message) =>
                new() { Success = false, Message = message };
        }

        public async Task<(long MaxFileBytes, int MaxRecords)> GetContactImportLimitsAsync(AppUserViewModel currentUser)
        {
            var subscription = await new OrganisationSubscriptionBusiness().GetOrganisationSubscriptionByOrgIdAsync(currentUser.OrganisationId);
            var plan = subscription is null ? null : await new SubscriptionPlanBusiness().GetSubscriptionPlanByIdAsync(subscription.PlanId);

            if (plan is null)
                return (1 * 1024 * 1024, 1000);   // no active subscription: Free limits

            return (plan.MaxImportFileMb * 1024L * 1024L, plan.MaxImportRecords);
        }

        #region PRIVATE METHODS
        private static async Task<ContactDetailsViewModel> BuildContactDetailsViewModel(Contact contact, string currentUserEmail)
        {
            var model = new ContactDetailsViewModel();
            if (contact is null)
                return model;

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);
            if (currentUser is null || contact.OrganisationId != currentUser.OrganisationId)
                return model;

            model.Contact = ObjectMapper.Mapper.Map<ContactViewModel>(contact);

            var orgId = currentUser.OrganisationId;

            var auditLogsTask = new AuditLogBusiness().GetByEntityAsync("Contact", contact.Id, orgId);
            var createdByTask = new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(contact.CreatedBy);
            var linkedAddressesTask = new AddressBusiness.AddressBusiness().GetEntityLinkedAddressesAsync(EntityTypes.Contact, contact.Id, orgId);
            var organisationTask = new OrganisationBusiness().GetOrganisationById(orgId);
            var companyTask = new CompanyBusiness.CompanyBusiness().GetCompanyByIdAsync(contact.CompanyId);

            await Task.WhenAll(auditLogsTask, createdByTask, linkedAddressesTask, organisationTask, companyTask);

            model.AuditLogs = auditLogsTask.Result;
            model.LinkedAddresses = linkedAddressesTask.Result;
            model.Organisaion = organisationTask.Result;
            model.Company = companyTask.Result ?? new ();

            var createdBy = createdByTask.Result;
            model.CreatedBy = createdBy;

            if (contact.UpdatedBy.HasValue)
            {
                model.UpdatedBy = contact.UpdatedBy.Value == createdBy.Id
                    ? createdBy
                    : await new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(contact.UpdatedBy.Value);
            }

            return model;
        }

        private static Dictionary<string, ImportFieldMapping>? ParseMapping(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, ImportFieldMapping>>(
                    json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (System.Text.Json.JsonException)
            {
                return null;
            }
        }

        private static async Task<byte[]> ReadAllBytesAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }

        private static async Task<int> CountCsvRecordsAsync(IFormFile file, string charset)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);   // needed for windows-1252
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding(charset));
            var text = await reader.ReadToEndAsync();

            var rows = 0;        // rows with a value, header included
            var blanks = 0;      // blank rows in a row
            var inQuotes = false;
            var hasValue = false;
            var prev = '\0';

            bool Flush()
            {
                var keepGoing = true;
                if (hasValue)
                {
                    if (blanks > 10) keepGoing = false;
                    else { rows++; blanks = 0; }
                }
                else blanks++;
                hasValue = false;
                return keepGoing;
            }

            foreach (var c in text)
            {
                if (inQuotes)
                {
                    if (c == '"') inQuotes = false;
                    else if (!char.IsWhiteSpace(c)) hasValue = true;
                }
                else if (c == '"') inQuotes = true;
                else if (c == '\r' || (c == '\n' && prev != '\r'))
                {
                    if (!Flush()) return Math.Max(rows - 1, 0);
                }
                else if (!char.IsWhiteSpace(c) && c != ',' && c != ';') hasValue = true;

                prev = c;
            }

            Flush();   // last row without a trailing line break
            return Math.Max(rows - 1, 0);   // header row excluded
        }
        #endregion
    }
}
