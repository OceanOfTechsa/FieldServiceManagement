using FieldServiceManagement.Data.DataModels.Address;

namespace FieldServiceManagement.Business.Imports.Readers
{
    // The organisation's addresses, indexed by street + city + postal code, so an import reuses an address
    // that already exists instead of creating a copy.
    public class ImportAddressBook
    {
        private readonly Dictionary<string, List<Address>> _index = new();
        private readonly Dictionary<Guid, Address> _byId = new();

        public ImportAddressBook(IEnumerable<Address> existing)
        {
            foreach (var a in existing.OrderBy(a => a.CreatedAt))   // oldest first, so the oldest copy is the one reused
                Add(a);
        }

        // Compares text ignoring case and extra spaces
        public static string Norm(string? s) =>
            string.Join(' ', (s ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();

        private static string Key(string? street, string? city, string? zip) => $"{Norm(street)}|{Norm(city)}|{Norm(zip)}";

        public void Add(Address a)
        {
            _byId[a.Id] = a;
            if (!a.IsActive || a.IsDeleted) return;

            var key = Key(a.Street, a.City, a.ZipCode);
            if (!_index.TryGetValue(key, out var list)) _index[key] = list = new List<Address>();
            list.Add(a);
        }

        public void Remove(Address a)
        {
            _byId.Remove(a.Id);
            if (_index.TryGetValue(Key(a.Street, a.City, a.ZipCode), out var list)) list.Remove(a);
        }

        // State and Country only count when the file gave them
        private static bool Same(Address x, ImportAddressData d) =>
            Norm(x.Street) == Norm(d.Street) && Norm(x.City) == Norm(d.City) && Norm(x.ZipCode) == Norm(d.Zip)
            && (d.StateId is null || x.StateId == d.StateId)
            && (d.CountryId is null || x.CountryId == d.CountryId);

        // Does this address already hold exactly what the file says?
        public bool Matches(Guid addressId, ImportAddressData d) =>
            _byId.TryGetValue(addressId, out var a) && !a.IsDeleted && Same(a, d);

        // An existing address with the same street, city, postal code (and state and country, when the file gave them)
        public Address? Find(ImportAddressData d) =>
            _index.TryGetValue(Key(d.Street, d.City, d.Zip), out var list) ? list.FirstOrDefault(x => Same(x, d)) : null;

        // A new address, already indexed so later rows can find it. The caller adds it to the batch.
        public Address Create(ImportAddressData d, Guid organisationId, Guid userId, DateTime now)
        {
            var address = new Address
            {
                Id = Guid.NewGuid(),
                OrganisationId = organisationId,
                Street = d.Street,
                City = d.City,
                ZipCode = d.Zip,
                StateId = d.StateId,
                CountryId = d.CountryId,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedById = userId,
                UpdatedById = userId
            };
            Add(address);
            return address;
        }
    }
}