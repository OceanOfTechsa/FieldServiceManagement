using FieldServiceManagement.Business.Imports.Readers;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Country;
using FieldServiceManagement.Data.DataModels.Entity;
using FieldServiceManagement.Data.DataModels.State;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Extensions;   // SaDateTime()
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;
using FieldServiceManagement.Data.DataModels.Note;

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
                    using var lookupRepo = new ImportRepository();

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

                    // The notes the companies already have, so a re-import does not add them again
                    var notes = new ImportNotes(
                        job.NotesCsv is null
                            ? new List<Note>()
                            : await lookupRepo.GetEntitiesAsync<Note>(n =>
                                n.OrganisationId == job.OrganisationId && n.EntityTypeId == (int)EntityTypes.Company && !n.IsDeleted),
                        (int)EntityTypes.Company, "company");

                    _logger.LogInformation("Lookups loaded: {States} states, {Countries} countries",
                        states.Count, countries.Count);

                    var errors = new ImportErrorReport();
                    int created = 0, updated = 0;
                    var now = DateTime.Now.SaDateTime();

                    // Company Id value in the Companies file -> the real company. The Addresses and Notes files use it to find their company.
                    var fileKeyMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

                    // ───── The existing companies, their address links and the organisation's addresses, loaded once and shared by every file ─────
                    var existingCompanies = await lookupRepo.GetEntitiesAsync<Company>(
                        c => c.OrganisationId == job.OrganisationId && !c.IsDeleted);

                    var existingLinks = await lookupRepo.GetEntitiesAsync<EntityAddress>(
                        l => l.OrganisationId == job.OrganisationId && l.EntityTypeId == (int)EntityTypes.Company && !l.IsDeleted);

                    var addressBook = new ImportAddressBook(await lookupRepo.GetEntitiesAsync<Address>(
                        a => a.OrganisationId == job.OrganisationId && !a.IsDeleted));

                    var byId = existingCompanies.ToDictionary(c => c.Id);
                    var byName = existingCompanies.GroupBy(c => Norm(c.Name)).ToDictionary(g => g.Key, g => g.ToList());

                    // The link rows of the companies: (company Id, address type) -> its active links. The slots the app reads from.
                    var links = existingLinks.GroupBy(l => (l.EntityId, l.AddressTypeId)).ToDictionary(g => g.Key, g => g.ToList());

                    // Rows waiting to be saved. They are saved 500 at a time.
                    var batch = new ImportBatch();
                    var pending = new List<(string File, int Number, string[] Cells, bool IsNew)>();

                    // ───── Helpers shared by the Companies and Addresses files ─────

                    // The value to write for an optional field when updating.
                    // null = leave the current value (not mapped, or empty while "Don't update empty values" is on).
                    string? Opt(ImportRow r, string key)
                    {
                        if (!r.IsMapped(key)) return null;
                        var value = r.Get(key);
                        return value == "" && job.DontUpdateEmpty ? null : value;
                    }

                    // The database columns have a maximum length, and a longer value would fail the whole batch of rows
                    static string? TooLong(string label, string value, int max) =>
                        value.Length > max ? $"{label} must be {max} characters or fewer." : null;

                    // The address the file describes: an existing identical address is reused, otherwise a new one is created
                    Guid ResolveAddress(ImportAddressData d)
                    {
                        var found = addressBook.Find(d);
                        if (found is not null) return found.Id;

                        var newAddress = addressBook.Create(d, job.OrganisationId, job.UserId, now);
                        batch.NewAddresses.Add(newAddress);
                        return newAddress.Id;
                    }

                    // Links the company to an address in its Service or Billing slot (a row in EntityAddresses)
                    void AddLink(Company company, int typeId, Guid addressId, bool isPrimary)
                    {
                        var link = new EntityAddress
                        {
                            Id = Guid.NewGuid(),
                            OrganisationId = job.OrganisationId,
                            EntityTypeId = (int)EntityTypes.Company,
                            EntityId = company.Id,
                            AddressId = addressId,
                            AddressTypeId = typeId,
                            IsPrimary = isPrimary,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = now,
                            CreatedById = job.UserId
                        };
                        batch.NewLinks.Add(link);

                        if (!links.TryGetValue((company.Id, typeId), out var list)) links[(company.Id, typeId)] = list = new List<EntityAddress>();
                        list.Add(link);
                    }

                    // Soft-deletes the links of a slot, the way UpdateCompany does
                    void RemoveLinks(Company company, int typeId)
                    {
                        if (!links.TryGetValue((company.Id, typeId), out var list)) return;

                        foreach (var link in list)
                        {
                            if (batch.NewLinks.Remove(link)) continue;   // added in this same batch: it was never saved, so just drop it

                            link.IsDeleted = true;
                            link.UpdatedAt = now;
                            link.UpdatedById = job.UserId;
                            if (!batch.UpdatedLinks.Contains(link)) batch.UpdatedLinks.Add(link);
                        }
                        links.Remove((company.Id, typeId));
                    }

                    // Puts the file's address in the company's Service or Billing slot. An address is never edited, only linked:
                    //   the slot already holds this exact address -> nothing changes
                    //   otherwise -> the old link is soft-deleted and the company is linked to the (existing or new) address
                    // The Service address must be applied before the Billing address.
                    // Returns false when there was nothing to change.
                    bool ApplyAddress(Company company, AddressTypes type, ImportAddressData d)
                    {
                        var typeId = (int)type;
                        var current = type == AddressTypes.Service ? company.ServiceAddressId : company.BillingAddressId;

                        if (current != Guid.Empty && addressBook.Matches(current, d))
                        {
                            if (links.ContainsKey((company.Id, typeId))) return false;

                            // the company has this address but no link row: repair it
                            AddLink(company, typeId, current, type == AddressTypes.Service || current != company.ServiceAddressId);
                            return true;
                        }

                        var addressId = ResolveAddress(d);
                        var isPrimary = type == AddressTypes.Service || addressId != company.ServiceAddressId;   // like CreateCompany: Billing is not primary when it is the Service address

                        RemoveLinks(company, typeId);
                        AddLink(company, typeId, addressId, isPrimary);

                        if (type == AddressTypes.Service) company.ServiceAddressId = addressId;
                        else company.BillingAddressId = addressId;
                        return true;
                    }

                    // Finds the company an Addresses row belongs to, the way the mapping screen said to ("id" or "name")
                    (Company? Company, string? Error) FindParent(string value, string? by)
                    {
                        if (by == "id")
                        {
                            if (fileKeyMap.TryGetValue(value, out var mapped) && byId.TryGetValue(mapped, out var viaFile))
                                return (viaFile, null);   // an Id from the Companies file of this import

                            return Guid.TryParse(value, out var gid) && byId.TryGetValue(gid, out var viaId)
                                ? (viaId, null)
                                : (null, $"Company Id '{value}' was not found.");
                        }

                        if (!byName.TryGetValue(Norm(value), out var matches) || matches.Count == 0)
                            return (null, $"Company '{value}' was not found.");

                        return matches.Count == 1
                            ? (matches[0], null)
                            : (null, $"Company name '{value}' matches more than one company. Match by Company Id instead.");
                    }

                    async Task FlushAsync()
                    {
                        if (pending.Count == 0) return;

                        try
                        {
                            using var saveRepo = new ImportRepository();   // a fresh connection per batch
                            await saveRepo.SaveBatchAsync(batch);
                            created += pending.Count(p => p.IsNew);
                            updated += pending.Count(p => !p.IsNew);
                        }
                        catch (Exception ex)
                        {
                            // The whole batch was rolled back: skip its rows, and forget its new records so later rows cannot match them.
                            // (Changes to companies that already existed stay in memory for the rest of this import.)
                            _logger.LogError(ex, "Saving a batch failed");

                            foreach (var c in batch.NewCompanies)
                            {
                                byId.Remove(c.Id);
                                if (byName.TryGetValue(Norm(c.Name), out var sameName)) sameName.Remove(c);
                                foreach (var key in fileKeyMap.Where(kv => kv.Value == c.Id).Select(kv => kv.Key).ToList())
                                    fileKeyMap.Remove(key);
                            }
                            foreach (var a in batch.NewAddresses) addressBook.Remove(a);
                            notes.Forget(batch.NewNotes);  
                            foreach (var l in batch.NewLinks)
                                if (links.TryGetValue((l.EntityId, l.AddressTypeId), out var slot)) slot.Remove(l);

                            foreach (var p in pending)
                                errors.Add(p.File, p.Number, "The record could not be saved.", p.Cells);
                        }

                        batch = new ImportBatch();
                        pending.Clear();
                    }

                    // ═════════════ COMPANIES FILE ═════════════
                    if (job.CompaniesCsv is not null && job.CompaniesMapping is not null)
                    {
                        int createdBefore = created, updatedBefore = updated, skippedBefore = errors.Count;

                        // How to find an existing company: "new" uses Skip existing based on, the others use Find existing based on
                        var findBy = (job.ImportOption == "new" ? job.SkipExistingBasedOn : job.FindExistingBasedOn)?.ToLowerInvariant();   // none | id | name
                        _logger.LogInformation("Matching existing companies by: {FindBy}", findBy ?? "none");

                        var companyTypeNames = string.Join(", ", System.Enum.GetNames(typeof(CompanyType)));

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

                            // 3b. maximum lengths of the database columns
                            var name = row.Get("name");
                            var phone = row.Get("phone");
                            var mobile = row.Get("mobile");

                            var lengthProblem = TooLong("Company Name", name, 255) ?? TooLong("Email", email, 255)
                                ?? TooLong("Phone", phone, 20) ?? TooLong("Mobile", mobile, 20) ?? TooLong("Website", website, 500);
                            if (lengthProblem is not null)
                            {
                                errors.Add("Companies", number, lengthProblem, cells);
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
                            var nameKey = Norm(name);

                            Company? existing = null;
                            var sameNameCount = 0;

                            if (findBy == "id")
                            {
                                if (fileId != "" && fileKeyMap.TryGetValue(fileId, out var earlierId)) byId.TryGetValue(earlierId, out existing);   // same Id earlier in this file
                                else if (Guid.TryParse(fileId, out var realId)) byId.TryGetValue(realId, out existing);
                            }
                            else if (findBy == "name" && byName.TryGetValue(nameKey, out var sameName))
                            {
                                sameNameCount = sameName.Count;
                                if (sameNameCount == 1) existing = sameName[0];
                            }

                            if (sameNameCount > 1)
                            {
                                errors.Add("Companies", number, $"More than one company is named '{name}'. Match by Company Id instead.", cells);
                                continue;
                            }
                            if (existing is not null && job.ImportOption == "new")
                            {
                                errors.Add("Companies", number, "A company with this Id or Name already exists.", cells);
                                continue;
                            }
                            if (existing is null && job.ImportOption == "update")
                            {
                                errors.Add("Companies", number, "No existing company was found for this Id or Name.", cells);
                                continue;
                            }

                            // 7. a new company needs a type and both addresses: the database does not allow them to be empty
                            if (existing is null)
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

                            // 8. build the change
                            Company company;

                            if (existing is not null)
                            {
                                // UPDATE: the mandatory fields always change, the optional ones follow "Don't update empty values"
                                company = existing;
                                company.Name = name;
                                company.Email = email;
                                company.Phone = phone;

                                if (Opt(row, "mobile") is { } newMobile) company.Mobile = newMobile == "" ? null : newMobile;
                                if (Opt(row, "website") is { } newWebsite) company.Website = newWebsite == "" ? null : newWebsite;
                                if (typeId is int newType) company.Type = newType;   // Type is not nullable: an empty cell leaves it alone
                                if (Opt(row, "isActive") is { } active) company.IsActive = ParseActive(active, company.IsActive);

                                // an address block with data relinks the company when it differs from the address it has
                                if (service.Any) ApplyAddress(company, AddressTypes.Service, service);
                                if (billing.Any) ApplyAddress(company, AddressTypes.Billing, billing);

                                company.UpdatedAt = now;
                                company.UpdatedBy = job.UserId;

                                // a company added in this same batch is already going in as new
                                if (!batch.NewCompanies.Contains(company) && !batch.UpdatedCompanies.Contains(company))
                                    batch.UpdatedCompanies.Add(company);
                            }
                            else
                            {
                                // CREATE
                                company = new Company
                                {
                                    Id = Guid.NewGuid(),
                                    OrganisationId = job.OrganisationId,
                                    Name = name,
                                    Email = email,
                                    Phone = phone,
                                    Mobile = mobile == "" ? null : mobile,
                                    Website = website == "" ? null : website,
                                    Type = typeId!.Value,
                                    IsActive = ParseActive(row.Get("isActive"), true),
                                    IsDeleted = false,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    CreatedBy = job.UserId,
                                    UpdatedBy = job.UserId
                                };

                                // the address rows (or reused ones) and the links the new company points to. Service first, then Billing.
                                ApplyAddress(company, AddressTypes.Service, service);
                                ApplyAddress(company, AddressTypes.Billing, billing);

                                batch.NewCompanies.Add(company);

                                // a later row with the same name or Id now finds this company
                                byId[company.Id] = company;
                                if (!byName.TryGetValue(nameKey, out var list)) byName[nameKey] = list = new List<Company>();
                                list.Add(company);
                            }

                            if (fileId != "") fileKeyMap[fileId] = company.Id;

                            pending.Add(("Companies", number, cells, existing is null));
                            if (pending.Count >= 500) await FlushAsync();
                        }

                        await FlushAsync();   // the last, smaller batch

                        _logger.LogInformation("Companies file: {Created} created, {Updated} updated, {Skipped} skipped",
                            created - createdBefore, updated - updatedBefore, errors.Count - skippedBefore);
                    }

                    // ═════════════ ADDRESSES FILE ═════════════
                    if (job.AddressesCsv is not null && job.AddressesMapping is not null)
                    {
                        int createdBefore = created, updatedBefore = updated, skippedBefore = errors.Count;

                        foreach (var (number, cells) in ImportCsv.Parse(job.AddressesCsv, job.AddressesCharset).Skip(1))
                        {
                            var row = new ImportRow(number, cells, job.AddressesMapping);

                            // 1. mandatory fields
                            var blank = new List<string>();
                            if (row.Get("company") == "") blank.Add("Company");
                            if (row.Get("addressType") == "") blank.Add("Address Type");
                            if (blank.Count > 0)
                            {
                                errors.Add("Addresses", number, $"Mandatory field(s) empty: {string.Join(", ", blank)}.", cells);
                                continue;
                            }

                            // 2. Service or Billing
                            var typeText = row.Get("addressType").ToLowerInvariant();
                            if (typeText != "service" && typeText != "billing")
                            {
                                errors.Add("Addresses", number, "Address Type must be Service or Billing.", cells);
                                continue;
                            }
                            var type = typeText == "service" ? AddressTypes.Service : AddressTypes.Billing;

                            // 3. the address itself
                            var address = addresses.Read(row, "", "Address");
                            if (!address.Any)
                            {
                                errors.Add("Addresses", number, "Mandatory field(s) empty: Street, City, Postal Code.", cells);
                                continue;
                            }
                            if (address.Error is not null)
                            {
                                errors.Add("Addresses", number, address.Error, cells);
                                continue;
                            }

                            // 4. the company it belongs to
                            var parent = FindParent(row.Get("company"), row.MatchBy("company"));
                            if (parent.Company is null)
                            {
                                errors.Add("Addresses", number, parent.Error!, cells);
                                continue;
                            }
                            var company = parent.Company;

                            // 5. put the address in the company's Service or Billing slot
                            var addressesBefore = batch.NewAddresses.Count;
                            var changed = ApplyAddress(company, type, address);
                            var isNew = batch.NewAddresses.Count > addressesBefore;   // a new address row was created

                            if (changed)
                            {
                                company.UpdatedAt = now;
                                company.UpdatedBy = job.UserId;

                                if (!batch.NewCompanies.Contains(company) && !batch.UpdatedCompanies.Contains(company))
                                    batch.UpdatedCompanies.Add(company);
                            }

                            pending.Add(("Addresses", number, cells, isNew));
                            if (pending.Count >= 500) await FlushAsync();
                        }

                        await FlushAsync();

                        _logger.LogInformation("Addresses file: {Created} created, {Updated} updated, {Skipped} skipped",
                            created - createdBefore, updated - updatedBefore, errors.Count - skippedBefore);
                    }

                    // ═════════════ NOTES FILE ═════════════
                    if (job.NotesCsv is not null && job.NotesMapping is not null)
                    {
                        int createdBefore = created, skippedBefore = errors.Count;

                        foreach (var (number, cells) in ImportCsv.Parse(job.NotesCsv, job.NotesCharset).Skip(1))
                        {
                            var row = new ImportRow(number, cells, job.NotesMapping);

                            // 1. the row's own fields
                            var n = notes.Read(row);
                            if (n.Error is not null)
                            {
                                errors.Add("Notes", number, n.Error, cells);
                                continue;
                            }

                            // 2. the company the note belongs to
                            var parent = FindParent(n.ParentValue, row.MatchBy("parent"));
                            if (parent.Company is null)
                            {
                                errors.Add("Notes", number, parent.Error!, cells);
                                continue;
                            }

                            // 3. the same note on the same company is not added twice
                            var note = notes.TryCreate(parent.Company.Id, n.Title, n.Content, job.OrganisationId, job.UserId, now);
                            if (note is null)
                            {
                                errors.Add("Notes", number, notes.DuplicateMessage, cells);
                                continue;
                            }

                            batch.NewNotes.Add(note);
                            pending.Add(("Notes", number, cells, true));
                            if (pending.Count >= 500) await FlushAsync();
                        }

                        await FlushAsync();

                        _logger.LogInformation("Notes file: {Created} created, {Skipped} skipped",
                            created - createdBefore, errors.Count - skippedBefore);
                    }

                    // ───── Error report and completion ─────
                    foreach (var (reason, count) in errors.Summary())
                        _logger.LogInformation("Skipped {Count}x: {Reason}", count, reason);

                    var errorFile = errors.Save(job.ImportId);
                    var message = errors.Count > 0 ? $"{errors.Count:N0} record(s) were skipped. See the error report." : null;

                    await history.CompleteAsync(job.ImportId, DataImportStatus.Completed,
                        Created: created, Updated: updated, Skipped: errors.Count, ErrorFilePath: errorFile, Message: message,
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
        private static string Norm(string? s) => ImportAddressBook.Norm(s);

        // Reads Active / Inactive style values. Anything unrecognised keeps the fallback.
        private static bool ParseActive(string Value, bool Fallback) => Value.Trim().ToLowerInvariant() switch
        {
            "true" or "yes" or "y" or "1" or "active" => true,
            "false" or "no" or "n" or "0" or "inactive" => false,
            _ => Fallback
        };

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