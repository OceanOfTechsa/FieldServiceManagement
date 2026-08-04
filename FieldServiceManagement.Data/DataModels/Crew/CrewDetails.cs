using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.Data.DataModels.Crew
{
    public class CrewDetails : BaseGuidPrimaryKey
    {
        public string Name { get; set; } = null!;
        public int? CrewSize { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid OrganisationId { get; set; }

        // Created by
        public Guid? CB_Id { get; set; }
        public string? CB_Name { get; set; }
        public string? CB_Surname { get; set; }
        public string? CB_Email { get; set; }
        public string? CB_AvatarUrl { get; set; }
        public bool? CB_IsActive { get; set; }

        // Updated by
        public Guid? UB_Id { get; set; }
        public string? UB_Name { get; set; }
        public string? UB_Surname { get; set; }
        public string? UB_Email { get; set; }
        public string? UB_AvatarUrl { get; set; }
        public bool? UB_IsActive { get; set; }

        // Audit log (one row per entry, or all null if none exist)
        public Guid? AuditLogId { get; set; }
        public string? AuditLogEntityName { get; set; }
        public string? AuditLogEntityId { get; set; }
        public string? AuditLogAction { get; set; }
        public string? AuditLogFieldName { get; set; }
        public string? AuditLogOldValue { get; set; }
        public string? AuditLogNewValue { get; set; }
        public string? AuditLogComment { get; set; }
        public Guid? AuditLogPerformedByUserId { get; set; }
        public string? AuditLogPerformedByName { get; set; }
        public string? AuditLogIpAddress { get; set; }
        public Guid? AuditLogOrganisationId { get; set; }
        public DateTime? AuditLogCreatedAt { get; set; }
        public string? AuditLogVisibleTo { get; set; }
        public bool? AuditLogShowComment { get; set; }
    }
}
