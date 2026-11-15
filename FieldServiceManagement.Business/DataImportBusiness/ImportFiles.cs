using FieldServiceManagement.ViewModels.Contact;   // ImportFieldMapping
using Microsoft.AspNetCore.Http;

namespace FieldServiceManagement.Business.Imports
{
    public static class ImportFiles
    {
        // IFormFile streams are disposed when the request ends, so the worker gets a copy of the bytes
        public static async Task<byte[]> ReadAllBytesAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            return stream.ToArray();
        }

        // The mapping JSON the screen sends: { "<fieldKey>": { column, index, replaceEmpty, matchBy } }. Null when it is missing or invalid.
        public static Dictionary<string, ImportFieldMapping>? ParseMapping(string? json)
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
    }
}