namespace FieldServiceManagement.Business.Imports.Readers
{
    // What one address block of a row holds after reading and checking it
    public sealed record ImportAddressData(bool Any, string? Error, string Street, string City, string Zip, int? StateId, int? CountryId);

    // Reads and checks the address block of an import row. Shared by every module whose records have addresses.
    public class ImportAddressReader
    {
        private readonly ImportLookup<int> _states;
        private readonly ImportLookup<int> _countries;
        private readonly Dictionary<int, int> _stateCountry;   // state Id -> the country it belongs to

        public ImportAddressReader(ImportLookup<int> states, ImportLookup<int> countries, Dictionary<int, int> stateCountry)
        {
            _states = states;
            _countries = countries;
            _stateCountry = stateCountry;
        }

        // prefix is "service" or "billing" inside a module's own file, and "" in a separate Addresses file.
        // Any = false means no address was given. Error is set when the block has a problem.
        public ImportAddressData Read(ImportRow r, string prefix, string label)
        {
            string K(string name) => prefix == "" ? name.ToLowerInvariant() : prefix + name;   // "street" or "serviceStreet"

            string street = r.Get(K("Street")), city = r.Get(K("City")), zip = r.Get(K("Zip")),
                   state = r.Get(K("State")), country = r.Get(K("Country"));

            if (street + city + zip + state + country == "")
                return new ImportAddressData(false, null, "", "", "", null, null);

            ImportAddressData Fail(string message) => new(true, $"{label}: {message}", "", "", "", null, null);

            var missing = new List<string>();
            if (street == "") missing.Add("Street");
            if (city == "") missing.Add("City");
            if (zip == "") missing.Add("Postal Code");
            if (missing.Count > 0) return Fail($"{string.Join(", ", missing)} required.");

            int? stateId = null, countryId = null;

            if (state != "")
            {
                var found = _states.Find("State", state, r.MatchBy(K("State")));
                if (!found.Ok) return Fail(found.Error!);
                stateId = found.Value;
            }
            if (country != "")
            {
                var found = _countries.Find("Country", country, r.MatchBy(K("Country")));
                if (!found.Ok) return Fail(found.Error!);
                countryId = found.Value;
            }

            // the state must belong to the country in the same address
            if (stateId is int sid && countryId is int cid && _stateCountry.TryGetValue(sid, out var owningCountryId) && owningCountryId != cid)
                return Fail($"State '{state}' does not belong to Country '{country}'.");

            return new ImportAddressData(true, null, street, city, zip, stateId, countryId);
        }
    }
}