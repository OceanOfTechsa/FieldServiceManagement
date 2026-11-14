using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FieldServiceManagement.ViewModels.Contact
{
    public class ContactImportRequest
    {
        public IFormFile? ContactsFile { get; set; } = default!;
        public IFormFile? AddressesFile { get; set; }
        public IFormFile? NotesFile { get; set; }

        public string? ContactsMapping { get; set; }
        public string? AddressesMapping { get; set; }
        public string? NotesMapping { get; set; }

        public string? ContactsCharset { get; set; }
        public string? AddressesCharset { get; set; }
        public string? NotesCharset { get; set; }

        public string ImportOption { get; set; } = "new";
        public string? SkipExistingBasedOn { get; set; }
        public string? FindExistingBasedOn { get; set; }
        public bool DontUpdateEmpty { get; set; }
    }

    public class ImportFieldMapping
    {
        public string? Column { get; set; }
        public int? Index { get; set; }
        public string? ReplaceEmpty { get; set; }
        public string? MatchBy { get; set; }
    }

    public class ContactImportJob
    {
        public Guid ImportId { get; set; } = Guid.NewGuid();
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;

        public string ImportOption { get; set; } = "new";
        public string? SkipExistingBasedOn { get; set; }
        public string? FindExistingBasedOn { get; set; }
        public bool DontUpdateEmpty { get; set; }

        public byte[]? ContactsCsv { get; set; }
        public byte[]? AddressesCsv { get; set; }
        public byte[]? NotesCsv { get; set; }

        public string ContactsCharset { get; set; } = "utf-8";
        public string AddressesCharset { get; set; } = "iso-8859-1";
        public string NotesCharset { get; set; } = "iso-8859-1";

        public Dictionary<string, ImportFieldMapping>? ContactsMapping { get; set; }
        public Dictionary<string, ImportFieldMapping>? AddressesMapping { get; set; }
        public Dictionary<string, ImportFieldMapping>? NotesMapping { get; set; }

        public string ContactsFileName { get; set; } = string.Empty;
        public string AddressesFileName { get; set; } = string.Empty;
        public string NotesFileName { get; set; } = string.Empty;
        public DateTime InitiatedAt { get; set; }
    }

    public class ContactImportStartResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Guid? ImportId { get; set; }
    }
}
