using System.Text;

namespace FieldServiceManagement.Business.Imports
{
    // Collects the skipped rows and their reasons, then writes them to a CSV the user can download
    public class ImportErrorReport
    {
        private readonly List<(string File, int Row, string Reason, string[] Cells)> _errors = new();

        public int Count => _errors.Count;
        // Groups the reasons so the worker can log "12x: Company 'X' was not found." instead of 12 separate lines
        public IEnumerable<(string Reason, int Count)> Summary() =>
            _errors.GroupBy(e => e.Reason)
                   .Select(g => (Reason: g.Key, Count: g.Count()))
                   .OrderByDescending(x => x.Count);

        public void Add(string File, int Row, string Reason, string[] Cells) => _errors.Add((File, Row, Reason, Cells));

        // Returns the file path, or null when there is nothing to report
        public string? Save(Guid ImportId)
        {
            if (_errors.Count == 0) return null;

            // App_Data is never served by the website. The file is only given out by DataImportController.Errors.
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "ImportErrors");
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, $"{ImportId}.csv");

            var sb = new StringBuilder("File,Row,Reason,Values\r\n");
            foreach (var e in _errors.OrderBy(e => e.File).ThenBy(e => e.Row))
                sb.Append($"{e.File},{e.Row},{Cell(e.Reason)},{Cell(string.Join(" | ", e.Cells))}\r\n");

            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));   // with a BOM so Excel reads accents correctly
            return path;
        }

        private static string Cell(string Value)
        {
            // a cell starting with = + - @ would run as a formula in Excel, so it gets an apostrophe in front
            if (Value.Length > 0 && "=+-@".Contains(Value[0])) Value = "'" + Value;
            return "\"" + Value.Replace("\"", "\"\"") + "\"";
        }
    }
}