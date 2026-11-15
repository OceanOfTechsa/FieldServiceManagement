using FieldServiceManagement.Business.Imports;
using FieldServiceManagement.Business.NotificationBusiness;
using FieldServiceManagement.Business.NotificationBusiness.ImportNotifications;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Data.DataModels.Country;
using FieldServiceManagement.Data.DataModels.Entity;
using FieldServiceManagement.Data.DataModels.Note;
using FieldServiceManagement.Data.DataModels.Notification;
using FieldServiceManagement.Data.DataModels.State;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Contact;
using FieldServiceManagement.ViewModels.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Text;

namespace FieldServiceManagement.Business.Imports.Workers
{
    public class ContactImportWorker : BackgroundService
    {
        private readonly ILogger<ContactImportWorker> _logger;

        public ContactImportWorker(ILogger<ContactImportWorker> logger)
        {
            _logger = logger;
        }

        // What one address block of a row holds after reading and checking it
        private sealed record AddressData(bool Any, string? Error, string Street, string City, string Zip, int? StateId, int? CountryId);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);   // lets the reader decode windows-1252 files

            using (var startup = new DataImportRepository())
            {
                await startup.FailActiveImportsAsync("The import was interrupted by a server restart. Please start it again.");
            }

            await foreach (var job in ImportQueue.Jobs.Reader.ReadAllAsync(stoppingToken))
            {
                using var history = new DataImportRepository();

                try
                {
                    _logger.LogInformation("Contact import {ImportId} started", job.ImportId);
                    await history.MarkRunningAsync(job.ImportId, DateTime.Now.SaDateTime());

                    // ───── Load the lookup data once ─────
                    using var lookupRepo = new ImportLookupRepository();

                    var companies = new ImportLookup<Guid>(await lookupRepo.GetLookupAsync<Company, Guid>(
                        c => c.OrganisationId == job.OrganisationId && !c.IsDeleted,
                        c => c.Id,
                        c => c.Name));

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

                    _logger.LogInformation("Lookups loaded: {Companies} companies, {States} states, {Countries} countries",
                        companies.Count, states.Count, countries.Count);

                    var errors = new ImportErrorReport();
                    var summaries = new List<ImportFileSummary>();
                    int created = 0, updated = 0;
                    var now = DateTime.Now.SaDateTime();

                    // Contact Id value in the Contacts file -> the real contact. Addresses and Notes files use it to find their contact.
                    var fileKeyMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

                    // ───── The existing contacts, addresses and address links, loaded once and shared by every file ─────
                    List<Contact> existingContacts;
                    List<EntityAddress> existingLinks;
                    List<Address> orgAddresses;
                    using (var readRepo = new ContactRepository())
                    {
                        existingContacts = await readRepo.GetContactsAsync(job.OrganisationId);
                        existingLinks = await readRepo.GetAddressLinksAsync(job.OrganisationId);
                        orgAddresses = await readRepo.GetAddressesAsync(job.OrganisationId);
                    }
                    var addressesById = orgAddresses.ToDictionary(a => a.Id);

                    var byId = existingContacts.ToDictionary(c => c.Id);
                    var byEmail = new Dictionary<string, Contact>(StringComparer.OrdinalIgnoreCase);
                    foreach (var c in existingContacts) byEmail[c.Email.Trim()] = c;
                    Dictionary<string, List<Guid>>? byName = null;   // built the first time a file matches contacts by name

                    // The link rows of the contacts: (contact Id, address type) -> link. The slots the app reads from.
                    var links = existingLinks
                        .Where(l => l.EntityTypeId == (int)EntityTypes.Contact)
                        .GroupBy(l => (l.EntityId, l.AddressTypeId))
                        .ToDictionary(g => g.Key, g => g.OrderByDescending(l => l.IsPrimary).ThenByDescending(l => l.CreatedAt).First());

                    // How many links use each address (any record, any type). More than one means the address is shared.
                    var linkCount = existingLinks.GroupBy(l => l.AddressId).ToDictionary(g => g.Key, g => g.Count());

                    // How to find an existing contact: "new" uses Skip existing based on, the others use Find existing based on
                    var findBy = (job.ImportOption == "new" ? job.SkipExistingBasedOn : job.FindExistingBasedOn)?.ToLowerInvariant();   // none | id | email
                    _logger.LogInformation("Matching existing contacts by: {FindBy}", findBy ?? "none");

                    // Rows waiting to be saved. They are saved 500 at a time.
                    var newContacts = new List<Contact>();
                    var updatedContacts = new Dictionary<Guid, Contact>();
                    var newAddresses = new List<Address>();
                    var updatedAddresses = new Dictionary<Guid, Address>();
                    var newLinks = new List<EntityAddress>();
                    var updatedLinks = new Dictionary<Guid, EntityAddress>();
                    var newNotes = new List<Note>();                 // NOTES
                    var noteKeys = new HashSet<string>();            // NOTES: every note already attached to a contact, plus the ones added by this import
                    var noteKeysInBatch = new List<string>();        // NOTES: the keys added by the batch that is waiting to be saved
                    var pending = new List<(string File, int Number, string[] Cells, bool IsNew)>();

                    // ───── Helpers shared by the Contacts, Addresses and Notes sections ─────

                    // Reads and checks one address block of a row.
                    // prefix is "service" or "billing" in the Contacts file, and "" in the Addresses file.
                    // Any = false means no address was given. Error is set when the block has a problem.
                    AddressData ReadAddress(ImportRow r, string prefix, string label)
                    {
                        string K(string name) => prefix == "" ? name.ToLowerInvariant() : prefix + name;   // "street" or "serviceStreet"

                        string street = r.Get(K("Street")), city = r.Get(K("City")), zip = r.Get(K("Zip")),
                               state = r.Get(K("State")), country = r.Get(K("Country"));

                        if (street + city + zip + state + country == "")
                            return new AddressData(false, null, "", "", "", null, null);

                        AddressData Fail(string message) => new(true, $"{label}: {message}", "", "", "", null, null);

                        var missing = new List<string>();
                        if (street == "") missing.Add("Street");
                        if (city == "") missing.Add("City");
                        if (zip == "") missing.Add("Postal Code");
                        if (missing.Count > 0) return Fail($"{string.Join(", ", missing)} required.");

                        int? stateId = null, countryId = null;

                        if (state != "")
                        {
                            var found = states.Find("State", state, r.MatchBy(K("State")));
                            if (!found.Ok) return Fail(found.Error!);
                            stateId = found.Value;
                        }
                        if (country != "")
                        {
                            var found = countries.Find("Country", country, r.MatchBy(K("Country")));
                            if (!found.Ok) return Fail(found.Error!);
                            countryId = found.Value;
                        }

                        // the state must belong to the country in the same address
                        if (stateId is int sid && countryId is int cid && stateCountry.TryGetValue(sid, out var owningCountryId) && owningCountryId != cid)
                            return Fail($"State '{state}' does not belong to Country '{country}'.");

                        return new AddressData(true, null, street, city, zip, stateId, countryId);
                    }

                    // The value to write for an optional field when updating.
                    // null = leave the current value (not mapped, or empty while "Don't update empty values" is on).
                    string? Opt(ImportRow r, string key)
                    {
                        if (!r.IsMapped(key)) return null;
                        var value = r.Get(key);
                        return value == "" && job.DontUpdateEmpty ? null : value;
                    }

                    // ── Finding an address or a note that already exists ──
                    // Compares text ignoring case and extra spaces
                    static string Norm(string? s) =>
                        string.Join(' ', (s ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();

                    static string AddrKey(string? street, string? city, string? zip) => $"{Norm(street)}|{Norm(city)}|{Norm(zip)}";

                    // NOTES: the same note on the same contact (ignoring case and extra spaces) has the same key
                    static string NoteKey(Guid entityId, string? title, string? content)
                    {
                        var hash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
                            Encoding.UTF8.GetBytes(Norm(title) + "\n" + Norm(content))));
                        return entityId + "|" + hash;
                    }

                    // Every active address in the organisation, grouped by street + city + postal code
                    var addressIndex = new Dictionary<string, List<Address>>();

                    void IndexAdd(Address x)
                    {
                        if (!x.IsActive || x.IsDeleted) return;
                        var key = AddrKey(x.Street, x.City, x.ZipCode);
                        if (!addressIndex.TryGetValue(key, out var list)) addressIndex[key] = list = new List<Address>();
                        list.Add(x);
                    }

                    // Call this BEFORE changing an address, while its old values are still in place
                    void IndexRemove(Address x)
                    {
                        if (addressIndex.TryGetValue(AddrKey(x.Street, x.City, x.ZipCode), out var list)) list.Remove(x);
                    }

                    foreach (var x in orgAddresses.OrderBy(x => x.CreatedAt)) IndexAdd(x);   // oldest first, so the oldest copy is the one reused

                    // NOTES: the notes the contacts already have, so a re-import does not add them again
                    if (job.NotesCsv is not null)
                    {
                        using var noteRepo = new ContactRepository();
                        foreach (var n in await noteRepo.GetNotesAsync(job.OrganisationId, (int)EntityTypes.Contact))
                            noteKeys.Add(NoteKey(n.EntityId, n.Title, n.Content));
                    }

                    // State and Country only count when the file gave them
                    bool SameAddress(Address x, AddressData a) =>
                        Norm(x.Street) == Norm(a.Street) && Norm(x.City) == Norm(a.City) && Norm(x.ZipCode) == Norm(a.Zip)
                        && (a.StateId is null || x.StateId == a.StateId)
                        && (a.CountryId is null || x.CountryId == a.CountryId);

                    Address? FindExistingAddress(AddressData a) =>
                        addressIndex.TryGetValue(AddrKey(a.Street, a.City, a.Zip), out var list)
                            ? list.FirstOrDefault(x => SameAddress(x, a))
                            : null;

                    Address NewAddress(AddressData a)
                    {
                        var address = new Address
                        {
                            Id = Guid.NewGuid(),
                            OrganisationId = job.OrganisationId,
                            Street = a.Street,
                            City = a.City,
                            ZipCode = a.Zip,
                            StateId = a.StateId,
                            CountryId = a.CountryId,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = now,
                            UpdatedAt = now,
                            CreatedById = job.UserId,
                            UpdatedById = job.UserId
                        };
                        newAddresses.Add(address);
                        addressesById[address.Id] = address;
                        IndexAdd(address);   // later rows can now find it
                        return address;
                    }

                    // Links the contact to an address in its Service or Billing slot (a row in EntityAddresses)
                    void AddLink(Contact contact, int typeId, Guid addressId)
                    {
                        var link = new EntityAddress
                        {
                            Id = Guid.NewGuid(),
                            OrganisationId = job.OrganisationId,
                            EntityTypeId = (int)EntityTypes.Contact,
                            EntityId = contact.Id,
                            AddressId = addressId,
                            AddressTypeId = typeId,
                            IsPrimary = true,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = now,
                            CreatedById = job.UserId
                        };
                        newLinks.Add(link);
                        links[(contact.Id, typeId)] = link;
                        linkCount[addressId] = linkCount.GetValueOrDefault(addressId) + 1;
                    }

                    // Soft-deletes a link, the way the app does
                    void RemoveLink(EntityAddress link)
                    {
                        if (!newLinks.Remove(link))   // a link created in this same batch was never saved: just drop it
                        {
                            link.IsDeleted = true;
                            link.UpdatedAt = now;
                            link.UpdatedById = job.UserId;
                            updatedLinks[link.Id] = link;
                        }
                        links.Remove((link.EntityId, link.AddressTypeId));
                        linkCount[link.AddressId] = Math.Max(0, linkCount.GetValueOrDefault(link.AddressId) - 1);
                    }

                    // Puts an address in the contact's Service or Billing slot. Returns the address Id.
                    //   1. the slot already holds this exact address -> nothing changes
                    //   2. the same address exists in the organisation -> it is reused, no new address is created
                    //   3. otherwise: updated in place if only this contact uses it, copied if it is shared, created if the slot was empty
                    // isNew is true only when the slot had no address before.
                    Guid ApplyAddress(Contact contact, AddressTypes type, AddressData a, out bool isNew)
                    {
                        var typeId = (int)type;
                        links.TryGetValue((contact.Id, typeId), out var link);

                        // the link table is what the app reads, the contact's own columns are the fallback
                        Guid? currentId = link?.AddressId ?? (type == AddressTypes.Service ? contact.ServiceAddressId : contact.BillingAddressId);

                        Address? current = null;
                        if (currentId is Guid id && addressesById.TryGetValue(id, out var found) && !found.IsDeleted)
                            current = found;

                        // 1. the slot already holds this exact address
                        if (current is not null && SameAddress(current, a))
                        {
                            if (link is null) AddLink(contact, typeId, current.Id);   // repairs a contact that had the address but no link
                            isNew = false;
                            return current.Id;
                        }

                        // 2. the same address already exists: reuse it
                        var match = FindExistingAddress(a);
                        if (match is not null)
                        {
                            if (link is not null) RemoveLink(link);
                            AddLink(contact, typeId, match.Id);
                            isNew = current is null;
                            return match.Id;
                        }

                        // 3. a genuinely new address for this slot
                        if (current is not null)
                        {
                            var usedElsewhere = linkCount.GetValueOrDefault(current.Id) - (link is not null ? 1 : 0) > 0;

                            if (!usedElsewhere)
                            {
                                IndexRemove(current);   // before the values change
                                current.Street = a.Street;
                                current.City = a.City;
                                current.ZipCode = a.Zip;
                                if (a.StateId is not null || !job.DontUpdateEmpty) current.StateId = a.StateId;
                                if (a.CountryId is not null || !job.DontUpdateEmpty) current.CountryId = a.CountryId;
                                current.UpdatedAt = now;
                                current.UpdatedById = job.UserId;
                                IndexAdd(current);

                                // an address added in this same batch is already going in as new
                                if (!newAddresses.Contains(current)) updatedAddresses[current.Id] = current;

                                if (link is null) AddLink(contact, typeId, current.Id);
                                isNew = false;
                                return current.Id;
                            }

                            // shared with another record: it is left alone and this contact gets its own copy
                            var copy = NewAddress(a);
                            if (link is not null) RemoveLink(link);
                            AddLink(contact, typeId, copy.Id);
                            isNew = false;
                            return copy.Id;
                        }

                        var fresh = NewAddress(a);
                        AddLink(contact, typeId, fresh.Id);
                        isNew = true;
                        return fresh.Id;
                    }

                    // Finds the contact an Addresses or Notes row belongs to, the way the mapping screen said to ("id", "email" or "name")
                    (Contact? Contact, string? Error) FindParent(string value, string? by)
                    {
                        switch (by)
                        {
                            case "id":
                                if (fileKeyMap.TryGetValue(value, out var mapped) && byId.TryGetValue(mapped, out var viaFile))
                                    return (viaFile, null);   // an Id from the Contacts file of this import
                                return Guid.TryParse(value, out var gid) && byId.TryGetValue(gid, out var viaId)
                                    ? (viaId, null)
                                    : (null, $"Contact Id '{value}' was not found.");

                            case "email":
                                return byEmail.TryGetValue(value, out var viaEmail)
                                    ? (viaEmail, null)
                                    : (null, $"Contact with email '{value}' was not found.");

                            default:
                                byName ??= byId.Values
                                    .GroupBy(c => $"{c.Name} {c.Surname}".Trim(), StringComparer.OrdinalIgnoreCase)
                                    .ToDictionary(g => g.Key, g => g.Select(c => c.Id).ToList(), StringComparer.OrdinalIgnoreCase);

                                if (!byName.TryGetValue(value, out var ids)) return (null, $"Contact '{value}' was not found.");
                                return ids.Count == 1
                                    ? (byId[ids[0]], null)
                                    : (null, $"Contact name '{value}' matches more than one contact. Match by Id or Email instead.");
                        }
                    }

                    async Task FlushAsync()
                    {
                        if (pending.Count == 0) return;

                        try
                        {
                            using var saveRepo = new ContactRepository();   // a fresh connection per batch
                            await saveRepo.SaveBatchAsync(newContacts, updatedContacts.Values.ToList(),
                                                          newAddresses, updatedAddresses.Values.ToList(),
                                                          newLinks, updatedLinks.Values.ToList(),
                                                          newNotes);
                            created += pending.Count(p => p.IsNew);
                            updated += pending.Count(p => !p.IsNew);
                        }
                        catch (Exception ex)
                        {
                            // The whole batch was rolled back: skip its rows, and forget its new records so later rows cannot match them.
                            // (Changes to records that already existed stay in memory for the rest of this import.)
                            _logger.LogError(ex, "Saving a batch failed");

                            foreach (var c in newContacts)
                            {
                                byId.Remove(c.Id);
                                if (byEmail.TryGetValue(c.Email, out var m) && m.Id == c.Id) byEmail.Remove(c.Email);
                                foreach (var key in fileKeyMap.Where(kv => kv.Value == c.Id).Select(kv => kv.Key).ToList())
                                    fileKeyMap.Remove(key);
                            }
                            foreach (var a in newAddresses) { addressesById.Remove(a.Id); IndexRemove(a); }
                            foreach (var l in newLinks)
                            {
                                if (links.TryGetValue((l.EntityId, l.AddressTypeId), out var current) && current == l)
                                    links.Remove((l.EntityId, l.AddressTypeId));
                                linkCount[l.AddressId] = Math.Max(0, linkCount.GetValueOrDefault(l.AddressId) - 1);
                            }
                            foreach (var k in noteKeysInBatch) noteKeys.Remove(k);   // NOTES: those notes were not saved, so an identical later row must not be skipped
                            byName = null;

                            foreach (var p in pending)
                                errors.Add(p.File, p.Number, "The record could not be saved.", p.Cells);
                        }

                        newContacts.Clear();
                        updatedContacts.Clear();
                        newAddresses.Clear();
                        updatedAddresses.Clear();
                        newLinks.Clear();
                        updatedLinks.Clear();
                        newNotes.Clear();
                        noteKeysInBatch.Clear();
                        pending.Clear();
                    }

                    // ═════════════ CONTACTS FILE ═════════════
                    if (job.ContactsCsv is not null && job.ContactsMapping is not null)
                    {
                        int createdBefore = created, updatedBefore = updated, skippedBefore = errors.Count;

                        var contactRows = ImportCsv.Parse(job.ContactsCsv, job.ContactsCharset).Skip(1);   // skip the header row

                        foreach (var (number, cells) in contactRows)
                        {
                            var row = new ImportRow(number, cells, job.ContactsMapping);

                            // 1. mandatory fields (after "Replace Empty Value")
                            var blank = new List<string>();
                            if (row.Get("name") == "") blank.Add("First Name");
                            if (row.Get("surname") == "") blank.Add("Last Name");
                            if (row.Get("email") == "") blank.Add("Email");
                            if (row.Get("company") == "") blank.Add("Company");
                            if (blank.Count > 0)
                            {
                                errors.Add("Contacts", number, $"Mandatory field(s) empty: {string.Join(", ", blank)}.", cells);
                                continue;
                            }

                            // 2. email format
                            var email = row.Get("email");
                            if (!MailAddress.TryCreate(email, out var parsed) || !parsed.Address.Equals(email, StringComparison.OrdinalIgnoreCase))
                            {
                                errors.Add("Contacts", number, "Invalid email address.", cells);
                                continue;
                            }

                            // 2b. salutation: the file has text (Mr, Mrs, Dr), the database stores the number
                            int? salutationId = null;
                            var salutationText = row.Get("salutation");
                            if (salutationText != "")
                            {
                                salutationId = ParseSalutation(salutationText);
                                if (salutationId is null)
                                {
                                    errors.Add("Contacts", number,
                                        $"Salutation '{salutationText}' is not valid. Use: {string.Join(", ", System.Enum.GetNames(typeof(UserSalutation)))}.", cells);
                                    continue;
                                }
                            }

                            // 3. company must exist
                            var company = companies.Find("Company", row.Get("company"), row.MatchBy("company"));
                            if (!company.Ok)
                            {
                                errors.Add("Contacts", number, company.Error!, cells);
                                continue;
                            }

                            // 4. service and billing address
                            var service = ReadAddress(row, "service", "Service Address");
                            if (service.Error is not null)
                            {
                                errors.Add("Contacts", number, service.Error, cells);
                                continue;
                            }
                            var billing = ReadAddress(row, "billing", "Billing Address");
                            if (billing.Error is not null)
                            {
                                errors.Add("Contacts", number, billing.Error, cells);
                                continue;
                            }

                            // 5. does this contact already exist, and what do the import options say to do?
                            var fileId = row.Get("id");
                            Contact? existing = null;
                            if (findBy == "id")
                            {
                                if (fileId != "" && fileKeyMap.TryGetValue(fileId, out var earlierId)) byId.TryGetValue(earlierId, out existing);   // same Id earlier in this file
                                else if (Guid.TryParse(fileId, out var realId)) byId.TryGetValue(realId, out existing);
                            }
                            else if (findBy == "email")
                            {
                                byEmail.TryGetValue(email, out existing);
                            }

                            if (existing is not null && job.ImportOption == "new")
                            {
                                errors.Add("Contacts", number, "A contact with this Id or Email already exists.", cells);
                                continue;
                            }
                            if (existing is null && job.ImportOption == "update")
                            {
                                errors.Add("Contacts", number, "No existing contact was found for this Id or Email.", cells);
                                continue;
                            }

                            // 6. build the change
                            Contact contact;

                            if (existing is not null)
                            {
                                // UPDATE: the mandatory fields always change, the optional ones follow "Don't update empty values"
                                contact = existing;
                                contact.Name = row.Get("name");
                                contact.Surname = row.Get("surname");
                                contact.Email = email;
                                contact.CompanyId = company.Value;

                                if (Opt(row, "phone") is { } phone) contact.Phone = phone;
                                if (Opt(row, "mobile") is { } mobile) contact.Mobile = mobile;
                                if (Opt(row, "isActive") is { } active) contact.IsActive = ParseActive(active, contact.IsActive);

                                if (row.IsMapped("salutation"))
                                {
                                    if (salutationId is int salutation) contact.Salutation = salutation;
                                    else if (!job.DontUpdateEmpty) contact.Salutation = 0;   // "Don't update empty values" is off: an empty cell clears it
                                }

                                // an address block with data updates (or creates) the contact's address in that slot
                                if (service.Any) contact.ServiceAddressId = ApplyAddress(contact, AddressTypes.Service, service, out _);
                                if (billing.Any) contact.BillingAddressId = ApplyAddress(contact, AddressTypes.Billing, billing, out _);

                                contact.UpdatedAt = now;
                                contact.UpdatedBy = job.UserId;

                                // a contact added in this same batch is already going in as new
                                if (!newContacts.Contains(contact)) updatedContacts[contact.Id] = contact;
                            }
                            else
                            {
                                // CREATE
                                contact = new Contact
                                {
                                    Id = Guid.NewGuid(),
                                    OrganisationId = job.OrganisationId,
                                    Name = row.Get("name"),
                                    Surname = row.Get("surname"),
                                    Email = email,
                                    Phone = row.Get("phone"),
                                    Mobile = row.Get("mobile"),
                                    CompanyId = company.Value,
                                    Salutation = salutationId ?? 0,
                                    IsActive = ParseActive(row.Get("isActive"), true),
                                    IsDeleted = false,
                                    CreatedAt = now,
                                    UpdatedAt = now,
                                    CreatedBy = job.UserId,
                                    UpdatedBy = job.UserId
                                };

                                // new address rows and links, which the new contact points to
                                if (service.Any) contact.ServiceAddressId = ApplyAddress(contact, AddressTypes.Service, service, out _);
                                if (billing.Any) contact.BillingAddressId = ApplyAddress(contact, AddressTypes.Billing, billing, out _);

                                newContacts.Add(contact);

                                // a later row with the same email or Id now finds this contact
                                byId[contact.Id] = contact;
                                byEmail[contact.Email] = contact;
                                byName = null;
                            }

                            if (fileId != "") fileKeyMap[fileId] = contact.Id;

                            pending.Add(("Contacts", number, cells, existing is null));
                            if (pending.Count >= 500) await FlushAsync();
                        }

                        await FlushAsync();   // the last, smaller batch

                        _logger.LogInformation("Contacts file: {Created} created, {Updated} updated, {Skipped} skipped",
                            created - createdBefore, updated - updatedBefore, errors.Count - skippedBefore);

                        summaries.Add(new ImportFileSummary("Contacts", job.ContactsFileName, contactRows.Count(),
                                created - createdBefore, updated - updatedBefore, errors.Count - skippedBefore));
                    }

                    // ═════════════ ADDRESSES FILE ═════════════
                    if (job.AddressesCsv is not null && job.AddressesMapping is not null)
                    {
                        int createdBefore = created, updatedBefore = updated, skippedBefore = errors.Count;

                        var addressRows = ImportCsv.Parse(job.AddressesCsv, job.AddressesCharset).Skip(1).ToList();

                        foreach (var (number, cells) in addressRows)
                        {
                            var row = new ImportRow(number, cells, job.AddressesMapping);

                            // 1. mandatory fields
                            var blank = new List<string>();
                            if (row.Get("contact") == "") blank.Add("Contact");
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
                            var address = ReadAddress(row, "", "Address");
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

                            // 4. the contact it belongs to
                            var parent = FindParent(row.Get("contact"), row.MatchBy("contact"));
                            if (parent.Contact is null)
                            {
                                errors.Add("Addresses", number, parent.Error!, cells);
                                continue;
                            }
                            var contact = parent.Contact;

                            // 5. put the address in the contact's Service or Billing slot
                            var addressId = ApplyAddress(contact, type, address, out var isNew);

                            if (type == AddressTypes.Service) contact.ServiceAddressId = addressId;
                            else contact.BillingAddressId = addressId;

                            contact.UpdatedAt = now;
                            contact.UpdatedBy = job.UserId;
                            if (!newContacts.Contains(contact)) updatedContacts[contact.Id] = contact;

                            pending.Add(("Addresses", number, cells, isNew));
                            if (pending.Count >= 500) await FlushAsync();
                        }

                        await FlushAsync();

                        _logger.LogInformation("Addresses file: {Created} created, {Updated} updated, {Skipped} skipped",
                            created - createdBefore, updated - updatedBefore, errors.Count - skippedBefore);

                        summaries.Add(new ImportFileSummary("Addresses", job.AddressesFileName, addressRows.Count,
                            created - createdBefore, updated - updatedBefore, errors.Count - skippedBefore));
                    }

                    // ═════════════ NOTES FILE ═════════════
                    if (job.NotesCsv is not null && job.NotesMapping is not null)
                    {
                        int createdBefore = created, skippedBefore = errors.Count;

                        var noteRows = ImportCsv.Parse(job.NotesCsv, job.NotesCharset).Skip(1).ToList();

                        foreach (var (number, cells) in noteRows)
                        {
                            var row = new ImportRow(number, cells, job.NotesMapping);

                            string parentValue = row.Get("parent"), title = row.Get("title"), content = row.Get("content");

                            // 1. mandatory fields (after "Replace Empty Value")
                            var blank = new List<string>();
                            if (parentValue == "") blank.Add("Parent");
                            if (content == "") blank.Add("Content");
                            if (blank.Count > 0)
                            {
                                errors.Add("Notes", number, $"Mandatory field(s) empty: {string.Join(", ", blank)}.", cells);
                                continue;
                            }

                            // 2. the title column is 200 characters
                            if (title.Length > 200)
                            {
                                errors.Add("Notes", number, "Title must be 200 characters or fewer.", cells);
                                continue;
                            }

                            // 3. the contact the note belongs to
                            var parent = FindParent(parentValue, row.MatchBy("parent"));
                            if (parent.Contact is null)
                            {
                                errors.Add("Notes", number, parent.Error!, cells);
                                continue;
                            }

                            // 4. the same note on the same contact is not added twice
                            var key = NoteKey(parent.Contact.Id, title, content);
                            if (!noteKeys.Add(key))
                            {
                                errors.Add("Notes", number, "This contact already has a note with the same title and content.", cells);
                                continue;
                            }
                            noteKeysInBatch.Add(key);

                            // 5. add the note
                            newNotes.Add(new Note
                            {
                                Id = Guid.NewGuid(),
                                OrganisationId = job.OrganisationId,
                                EntityTypeId = (int)EntityTypes.Contact,
                                EntityId = parent.Contact.Id,
                                Title = title == "" ? null : title,
                                Content = content,
                                IsSharedToClient = false,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedAt = now,
                                CreatedBy = job.UserId
                            });

                            pending.Add(("Notes", number, cells, true));
                            if (pending.Count >= 500) await FlushAsync();
                        }

                        await FlushAsync();

                        _logger.LogInformation("Notes file: {Created} created, {Skipped} skipped",
                            created - createdBefore, errors.Count - skippedBefore);

                        summaries.Add(new ImportFileSummary("Notes", job.NotesFileName, noteRows.Count,
                            created - createdBefore, 0, errors.Count - skippedBefore));
                    }

                    // ───── Error report and completion ─────
                    foreach (var (reason, count) in errors.Summary())
                        _logger.LogInformation("Skipped {Count}x: {Reason}", count, reason);

                    var errorFile = errors.Save(job.ImportId);
                    var message = errors.Count > 0 ? $"{errors.Count:N0} record(s) were skipped. See the error report." : null;

                    await history.CompleteAsync(job.ImportId, DataImportStatus.Completed,
                        Created: created, Updated: updated, Skipped: errors.Count, ErrorFilePath: errorFile, Message: message,
                        CompletedAt: DateTime.Now.SaDateTime());

                    await NotifyAsync(job, DataImportStatus.Completed, summaries);

                    _logger.LogInformation("Contact import {ImportId} completed", job.ImportId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Contact import {ImportId} failed", job.ImportId);

                    await history.CompleteAsync(job.ImportId, DataImportStatus.Failed,
                        Created: 0, Updated: 0, Skipped: 0, ErrorFilePath: null,
                        Message: "The import failed unexpectedly. Please try again or contact support.",
                        CompletedAt: DateTime.Now.SaDateTime());

                    await NotifyAsync(job, DataImportStatus.Failed, new List<ImportFileSummary>());
                }
            }
        }

        // Reads Active / Inactive style values. Anything unrecognised keeps the fallback.
        private static bool ParseActive(string Value, bool Fallback) => Value.Trim().ToLowerInvariant() switch
        {
            "true" or "yes" or "y" or "1" or "active" => true,
            "false" or "no" or "n" or "0" or "inactive" => false,
            _ => Fallback
        };

        // "Mr", "mrs", "Dr." -> the UserSalutation number. Returns null when it is not a known salutation.
        private static int? ParseSalutation(string Value)
        {
            var text = Value.Trim().TrimEnd('.');

            if (System.Enum.TryParse<UserSalutation>(text, true, out var salutation) &&
                System.Enum.IsDefined(typeof(UserSalutation), salutation))
                return (int)salutation;

            return null;
        }

        // Sends the summary email and the in-app notification. A problem in either is logged and never changes the import's result.
        private async Task NotifyAsync(ContactImportJob job, DataImportStatus status, List<ImportFileSummary> files)
        {
            var succeeded = status == DataImportStatus.Completed;
            var skipped = files.Sum(f => f.Skipped);

            // 1. email
            try
            {
                var user = await new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(job.UserId);
                var organisation = await new OrganisationBusiness().GetOrganisationById(job.OrganisationId);

                new DataImportCompletedNotification(job.UserEmail, $"{user.Name} {user.Surname}".Trim(),
                    organisation?.Name ?? "", "Contacts", job.InitiatedAt, status, files).SendNotification();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send the import summary email for {ImportId}", job.ImportId);
            }

            // 2. in-app notification for the user who started the import
            try
            {
                // the time the import was started, the same one the email and the history page show
                var initiatedText = $"{job.InitiatedAt:MMM dd, yyyy hh:mm} {job.InitiatedAt:tt}";

                var successMsg = $"The data import you initiated at {initiatedText} has been completed. " +
                                 "You can view the details using the link below or under Import History on the Data Import page in Setup." +
                                 (skipped > 0 ? "\r\n\r\nSome records were skipped. The error report lists each skipped row and the reason." : "");

                var errorMsg = $"The data import you initiated at {initiatedText} failed. Please check your files and try again. " +
                               "You can view the details using the link below or under Import History on the Data Import page in Setup.";

                await new UserNotificationBusiness().CreateUserNotificationAsync(new CreateUserNotification
                {
                    UserId = job.UserId,
                    OrganisationId = job.OrganisationId,
                    Type = "data_import",
                    Title = succeeded ? "Contacts import completed" : "Contacts import failed",
                    Message = succeeded ? successMsg : errorMsg,
                    RelatedEntityType = "DataImport",
                    RelatedEntityId = job.ImportId,
                    Severity = succeeded ? (byte)1 : (byte)2,
                    ActionText = "View import history",
                    ActionUrl = "/ImportHistory?module=Contacts",
                    CreatedById = job.UserId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not create the in-app notification for {ImportId}", job.ImportId);
            }
        }
    }
}

