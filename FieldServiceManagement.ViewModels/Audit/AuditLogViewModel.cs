using FieldServiceManagement.Data.DataModels.BaseClass;
using FieldServiceManagement.ViewModels.Extensions;

namespace FieldServiceManagement.ViewModels.Audit
{
    public class AuditLogViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Comment { get; set; }
        public Guid PerformedByUserId { get; set; }
        public string? PerformedByName { get; set; }
        public string? IpAddress { get; set; }
        public Guid? OrganisationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.SaDateTime();
        public bool IsDeleted { get; set; } = false;

        public bool? ShowEntityType { get; set; } = false; 

        public string? VisibleTo { get; set; }
        public bool ShowComment { get; set; }
        public string? createdByEmail { get; set; }
    }
}
