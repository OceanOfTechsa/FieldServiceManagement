using FieldServiceManagement.Business.Imports.Readers;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Country;
using FieldServiceManagement.Data.DataModels.State;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Extensions;   // SaDateTime()
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;

namespace FieldServiceManagement.Business.Imports.Workers
{
    public class CompanyImportWorker : BackgroundService
    {
        private readonly ILogger<CompanyImportWorker> _logger;

        public CompanyImportWorker(ILogger<CompanyImportWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);   // lets the reader decode windows-1252 files

            // No restart clean-up here: ContactImportWorker already fails every Queued or Running import
            // (for all modules) when the app starts, because the queues live in memory.

            await foreach (var job in ImportQueue.CompanyJobs.Reader.ReadAllAsync(stoppingToken))
            {
                using var history = new DataImportRepository();

                try
                {
                    _logger.LogInformation("Company import {ImportId} started", job.ImportId);
                    await history.MarkRunningAsync(job.ImportId, DateTime.Now.SaDateTime());

                    // ───── Load the lookup data once ─────
                    using var lookupRepo = new ImportLookupRepository();

                    var states = new ImportLookup<int>(await lookupRepo.GetLookupAsync<State, int>(
                        s => s.IsActive,
                        s => s.Id,
                        s => s.Name));

                    var countries = new ImportLookup<int>(await lookupRepo.GetLookupAsync<Country, int>(
                        c => c.IsActive,
                        c => c.Id,
                        c => c.Name));

                    // state Id -> the country it belongs to, for the "state matches country" check
                    var stateCountry = await lookupRepo.GetMapAsync<State, int, int>(
                        s => s.IsActive,
                        s => s.Id,
                        s => s.CountryId);

                    var addresses = new ImportAddressReader(states, countries, stateCountry);

                    _logger.LogInformation("Lookups loaded: {States} states, {Countries} countries",
                        states.Count, countries.Count);

                    var errors = new ImportErrorReport();

                    // ═════════════ COMPANIES FILE: check every row (nothing is saved yet) ═════════════
                    if (job.CompaniesCsv is not null && job.CompaniesMapping is not null)
                    {
                        // The companies that already exist, as (Id, Name)
                        var existing = await lookupRepo.GetLookupAsync<Company, Guid>(
                            c => c.OrganisationId == job.OrganisationId && !c.IsDeleted,
                            c => c.Id,
                            c => c.Name);

                        var existingIds = existing.Select(c => c.Id.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase);

                        // how many companies have each name, so a name used twice is not matched by mistake
                        var nameCount = new Dictionary<string, int>();
                        foreach (var c in existing)
                            nameCount[Norm(c.Name)] = nameCount.GetValueOrDefault(Norm(c.Name)) + 1;

                        // How to find an existing company: "new" uses Skip existing based on, the others use Find existing based on
                        var findBy = (job.ImportOption == "new" ? job.SkipExistingBasedOn : job.FindExistingBasedOn)?.ToLowerInvariant();   // none | id | name
                        _logger.LogInformation("Matching existing companies by: {FindBy}", findBy ?? "none");

                        var companyTypeNames = string.Join(", ", System.Enum.GetNames(typeof(CompanyType)));
                        int willCreate = 0, willUpdate = 0;

                        foreach (var (number, cells) in ImportCsv.Parse(job.CompaniesCsv, job.CompaniesCharset).Skip(1))   // skip the header row
                        {
                            var row = new ImportRow(number, cells, job.CompaniesMapping);

                            // 1. mandatory fields (after "Replace Empty Value")
                            var blank = new List<string>();
                            if (row.Get("name") == "") blank.Add("Company Name");
                            if (row.Get("email") == "") blank.Add("Email");
                            if (row.Get("phone") == "") blank.Add("Phone");
                            if (blank.Count > 0)
                            {
                                errors.Add("Companies", number, $"Mandatory field(s) empty: {string.Join(", ", blank)}.", cells);
                                continue;
                            }

                            // 2. email format
                            var email = row.Get("email");
                            if (!MailAddress.TryCreate(email, out var parsed) || !parsed.Address.Equals(email, StringComparison.OrdinalIgnoreCase))
                            {
                                errors.Add("Companies", number, "Invalid email address.", cells);
                                continue;
                            }

                            // 3. website: a full http(s) address, like the form's [Url] check
                            var website = row.Get("website");
                            if (website != "" && !(Uri.TryCreate(website, UriKind.Absolute, out var uri)
                                                  && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
                            {
                                errors.Add("Companies", number, "Website must be a valid address starting with http:// or https://.", cells);
                                continue;
                            }

                            // 4. company type: the file has text (Customer, Supplier), the database stores the number
                            int? typeId = null;
                            var typeText = row.Get("type");
                            if (typeText != "")
                            {
                                typeId = ParseCompanyType(typeText);
                                if (typeId is null)
                                {
                                    errors.Add("Companies", number, $"Company Type '{typeText}' is not valid. Use: {companyTypeNames}.", cells);
                                    continue;
                                }
                            }

                            // 5. service and billing address
                            var service = addresses.Read(row, "service", "Service Address");
                            if (service.Error is not null)
                            {
                                errors.Add("Companies", number, service.Error, cells);
                                continue;
                            }
                            var billing = addresses.Read(row, "billing", "Billing Address");
                            if (billing.Error is not null)
                            {
                                errors.Add("Companies", number, billing.Error, cells);
                                continue;
                            }

                            // 6. does this company already exist, and what do the import options say to do?
                            var fileId = row.Get("id");
                            var nameKey = Norm(row.Get("name"));

                            var exists = findBy switch
                            {
                                "id" => fileId != "" && existingIds.Contains(fileId),
                                "name" => nameCount.ContainsKey(nameKey),
                                _ => false   // "none": never treated as existing
                            };

                            if (exists && job.ImportOption == "new")
                            {
                                errors.Add("Companies", number, "A company with this Id or Name already exists.", cells);
                                continue;
                            }
                            if (!exists && job.ImportOption == "update")
                            {
                                errors.Add("Companies", number, "No existing company was found for this Id or Name.", cells);
                                continue;
                            }
                            if (exists && findBy == "name" && nameCount[nameKey] > 1)
                            {
                                errors.Add("Companies", number, $"More than one company is named '{row.Get("name")}'. Match by Company Id instead.", cells);
                                continue;
                            }

                            // 7. a new company needs a type and both addresses: the database does not allow them to be empty
                            if (!exists)
                            {
                                if (typeId is null)
                                {
                                    errors.Add("Companies", number, "Company Type is required to add a new company.", cells);
                                    continue;
                                }
                                if (!service.Any)
                                {
                                    errors.Add("Companies", number, "Service Address: Street, City, Postal Code required.", cells);
                                    continue;
                                }
                                if (!billing.Any)
                                {
                                    errors.Add("Companies", number, "Billing Address: Street, City, Postal Code required.", cells);
                                    continue;
                                }
                            }

                            if (exists)
                            {
                                willUpdate++;
                            }
                            else
                            {
                                willCreate++;
                                // a later row with the same name or Id now counts as existing
                                nameCount[nameKey] = 1;
                                if (fileId != "") existingIds.Add(fileId);
                            }
                        }

                        _logger.LogInformation("Companies file: would create {Create}, would update {Update}, {Skipped} skipped",
                            willCreate, willUpdate, errors.Count);
                    }

                    // ───── The Addresses and Notes files are only read for now ─────
                    if (job.AddressesCsv is not null)
                    {
                        var rows = ImportCsv.Parse(job.AddressesCsv, job.AddressesCharset);
                        _logger.LogInformation("Addresses file: {Count} data rows", rows.Count - 1);
                    }
                    if (job.NotesCsv is not null)
                    {
                        var rows = ImportCsv.Parse(job.NotesCsv, job.NotesCharset);
                        _logger.LogInformation("Notes file: {Count} data rows", rows.Count - 1);
                    }

                    // ───── Error report and completion ─────
                    foreach (var (reason, count) in errors.Summary())
                        _logger.LogInformation("Skipped {Count}x: {Reason}", count, reason);

                    var errorFile = errors.Save(job.ImportId);
                    var message = errors.Count > 0 ? $"{errors.Count:N0} record(s) were skipped. See the error report." : null;

                    await history.CompleteAsync(job.ImportId, DataImportStatus.Completed,
                        Created: 0, Updated: 0, Skipped: errors.Count, ErrorFilePath: errorFile, Message: message,
                        CompletedAt: DateTime.Now.SaDateTime());

                    _logger.LogInformation("Company import {ImportId} completed", job.ImportId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Company import {ImportId} failed", job.ImportId);

                    await history.CompleteAsync(job.ImportId, DataImportStatus.Failed,
                        Created: 0, Updated: 0, Skipped: 0, ErrorFilePath: null,
                        Message: "The import failed unexpectedly. Please try again or contact support.",
                        CompletedAt: DateTime.Now.SaDateTime());
                }
            }
        }

        // Compares text ignoring case and extra spaces
        private static string Norm(string? s) =>
            string.Join(' ', (s ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();

        // "Customer", "supplier" -> the CompanyType number. Returns null when it is not a known company type.
        private static int? ParseCompanyType(string Value)
        {
            var text = Value.Trim();
            if (text.Equals("Integrator", StringComparison.OrdinalIgnoreCase)) text = "Intergrator";   // the enum is spelled Intergrator

            if (System.Enum.TryParse<CompanyType>(text, true, out var type) &&
                System.Enum.IsDefined(typeof(CompanyType), type))   // stops numbers like "99" getting through
                return (int)type;

            return null;
        }
    }
}