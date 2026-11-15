using FieldServiceManagement.ViewModels.Contact;
using Microsoft.AspNetCore.Http;

namespace FieldServiceManagement.ViewModels.Company
{
    public class CompanyImportRequest
    {
        public IFormFile? CompaniesFile { get; set; }
        public IFormFile? AddressesFile { get; set; }
        public IFormFile? NotesFile { get; set; }

        // JSON: { "<fieldKey>": { column, index, replaceEmpty, matchBy } }
        public string? CompaniesMapping { get; set; }
        public string? AddressesMapping { get; set; }
        public string? NotesMapping { get; set; }

        public string? CompaniesCharset { get; set; }   // utf-8 | iso-8859-1 | windows-1252
        public string? AddressesCharset { get; set; }
        public string? NotesCharset { get; set; }

        public string ImportOption { get; set; } = "new";      // new | update | both
        public string? SkipExistingBasedOn { get; set; }       // none | id | name   (used by "new")
        public string? FindExistingBasedOn { get; set; }       // id | name           (used by "update" / "both")
        public bool DontUpdateEmpty { get; set; }
    }

    // Everything the background worker needs. Files are copied to byte[] because IFormFile
    // streams are disposed as soon as the request ends.
    public class CompanyImportJob
    {
        public Guid ImportId { get; set; } = Guid.NewGuid();
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime InitiatedAt { get; set; }

        public string ImportOption { get; set; } = "new";
        public string? SkipExistingBasedOn { get; set; }
        public string? FindExistingBasedOn { get; set; }
        public bool DontUpdateEmpty { get; set; }

        public byte[]? CompaniesCsv { get; set; }
        public byte[]? AddressesCsv { get; set; }
        public byte[]? NotesCsv { get; set; }

        public Dictionary<string, ImportFieldMapping>? CompaniesMapping { get; set; }
        public Dictionary<string, ImportFieldMapping>? AddressesMapping { get; set; }
        public Dictionary<string, ImportFieldMapping>? NotesMapping { get; set; }

        public string CompaniesCharset { get; set; } = "utf-8";
        public string AddressesCharset { get; set; } = "iso-8859-1";
        public string NotesCharset { get; set; } = "iso-8859-1";

        public string CompaniesFileName { get; set; } = string.Empty;
        public string AddressesFileName { get; set; } = string.Empty;
        public string NotesFileName { get; set; } = string.Empty;
    }
}