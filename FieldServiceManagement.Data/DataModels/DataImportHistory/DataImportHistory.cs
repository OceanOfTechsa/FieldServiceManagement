using FieldServiceManagement.Data.DataModels.BaseClass;
using FieldServiceManagement.Enum;

namespace FieldServiceManagement.Data.DataModels.DataImportHistory
{
    public class DataImportHistory: BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }

        public string UserEmail { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;

        public DataImportStatus StatusId { get; set; } = DataImportStatus.Queued;

        public string? ImportOption { get; set; }
        public string? FilesJson { get; set; }

        public int TotalRecords { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int SkippedCount { get; set; }

        public string? ErrorFilePath { get; set; }
        public string? Message { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
    }
}
