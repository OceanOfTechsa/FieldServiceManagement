using System.Text;

namespace FieldServiceManagement.Business.Imports
{
    // Turns the uploaded file bytes into rows of cells.
    // It lives in its own file because every import (Contacts now, other modules later) needs the same reader.
    public static class ImportCsv
    {
        // Returns every non-blank row as (row number, cells).
        // Row numbers count the blank rows too, so they match the spreadsheet: the header is row 1.
        public static List<(int Row, string[] Cells)> Parse(byte[] Bytes, string Charset)
        {
            var text = Encoding.GetEncoding(Charset).GetString(Bytes);
            if (text.Length > 0 && text[0] == '\uFEFF') text = text[1..];   // byte order mark

            // The delimiter is whichever of , ; tab appears most in the first line (same as the mapping screen)
            var firstLine = text.Split('\n', 2)[0];
            var delimiter = new[] { ',', ';', '\t' }.OrderByDescending(d => firstLine.Count(c => c == d)).First();

            var records = new List<string[]>();
            var row = new List<string>();
            var field = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }   // "" is a quote inside a quoted cell
                        else inQuotes = false;
                    }
                    else field.Append(c);   // commas and line breaks inside quotes are part of the cell
                }
                else if (c == '"') inQuotes = true;
                else if (c == delimiter) { row.Add(field.ToString()); field.Clear(); }
                else if (c == '\n' || c == '\r')
                {
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;   // \r\n counts once
                    row.Add(field.ToString()); field.Clear();
                    records.Add(row.ToArray()); row.Clear();
                }
                else field.Append(c);
            }

            if (field.Length > 0 || row.Count > 0) { row.Add(field.ToString()); records.Add(row.ToArray()); }   // last row with no line break

            var result = new List<(int, string[])>();
            var blanks = 0;

            for (var n = 0; n < records.Count; n++)
            {
                if (records[n].All(string.IsNullOrWhiteSpace)) { blanks++; continue; }
                if (blanks > 10) break;   // Zoho rule: more than 10 blank rows in a row ends the file
                blanks = 0;
                result.Add((n + 1, records[n]));
            }

            return result;
        }
    }
}