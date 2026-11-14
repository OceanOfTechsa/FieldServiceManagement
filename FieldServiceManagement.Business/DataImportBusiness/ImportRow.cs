using FieldServiceManagement.ViewModels.Contact;   // where ImportFieldMapping lives

namespace FieldServiceManagement.Business.Imports
{
    // One CSV row plus the user's mapping, so the rest of the code can ask for a value by field name
    // instead of working out column numbers. Any module's import can use it.
    public class ImportRow
    {
        private readonly Dictionary<string, ImportFieldMapping> _map;

        public int Number { get; }          // row number in the spreadsheet (the header is row 1)
        public string[] Cells { get; }

        public ImportRow(int Number, string[] Cells, Dictionary<string, ImportFieldMapping> Map)
        {
            this.Number = Number;
            this.Cells = Cells;
            _map = Map;
        }

        public bool IsMapped(string Key) => _map.TryGetValue(Key, out var m) && m.Index is not null;

        // The value for a field. Empty cells use the "Replace Empty Value" the user typed, if any.
        public string Get(string Key)
        {
            if (!_map.TryGetValue(Key, out var m) || m.Index is null) return "";

            var value = m.Index.Value < Cells.Length ? Cells[m.Index.Value].Trim() : "";
            return value.Length == 0 ? (m.ReplaceEmpty ?? "").Trim() : value;
        }

        // "id" or "name" or "email": the match-by choice from the mapping screen
        public string? MatchBy(string Key) =>
            _map.TryGetValue(Key, out var m) ? m.MatchBy?.ToLowerInvariant() : null;
    }
}