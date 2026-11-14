namespace FieldServiceManagement.Business.Imports
{
    public class ImportLookup<T>
    {
        private readonly Dictionary<string, T> _byId;
        private readonly Dictionary<string, List<T>> _byName;

        public int Count { get; }

        public ImportLookup(List<(T Id, string Name)> Items)
        {
            Count = Items.Count;

            _byId = Items.ToDictionary(x => x.Id!.ToString()!, x => x.Id, StringComparer.OrdinalIgnoreCase);

            // a name can belong to more than one record, so each name keeps a list
            _byName = Items
                .GroupBy(x => x.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToList(), StringComparer.OrdinalIgnoreCase);
        }

        // MatchBy is the "Id" / "Name" the user chose on the mapping screen
        public (bool Ok, T Value, string? Error) Find(string Label, string Value, string? MatchBy)
        {
            Value = Value.Trim();

            if (MatchBy == "id")
            {
                return _byId.TryGetValue(Value, out var id)
                    ? (true, id, null)
                    : (false, default!, $"{Label} '{Value}' was not found.");
            }

            if (!_byName.TryGetValue(Value, out var matches))
                return (false, default!, $"{Label} '{Value}' was not found.");

            return matches.Count == 1
                ? (true, matches[0], null)
                : (false, default!, $"{Label} '{Value}' matches more than one record. Match by Id instead.");
        }
    }
}