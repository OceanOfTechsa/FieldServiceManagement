namespace FieldServiceManagement.Data.DataModels.Company
{
    public class CompanyDetails
    {
        // Company
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Mobile { get; set; }
        public string? Website { get; set; }
        public int Type { get; set; }
        public Guid ServiceAddressId { get; set; }
        public Guid BillingAddressId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid OrganisationId { get; set; }
        public string? ServiceAddressName { get; set; }
        public string? BillingAddressName { get; set; }

        // CreatedBy
        public Guid CreatedById { get; set; }
        public Guid CreatedByOrganisationId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string CreatedBySurname { get; set; } = string.Empty;
        public string CreatedByEmail { get; set; } = string.Empty;
        public string CreatedByPhone { get; set; } = string.Empty;
        public string? CreatedByAvatarUrl { get; set; }
        public int? CreatedByUserRoleId { get; set; }
        public int? CreatedByPreferredLanguageId { get; set; }
        public bool CreatedByIsActive { get; set; }
        public int? CreatedByStatusId { get; set; }
        public bool CreatedByIsOwner { get; set; }
        public Guid CreatedByUserId { get; set; }
        public bool CreatedByIsDeleted { get; set; }
        public DateTime CreatedByCreatedAt { get; set; }
        public DateTime? CreatedByUpdatedAt { get; set; }
        public Guid CreatedByCreatedById { get; set; }
        public Guid? CreatedByUpdatedById { get; set; }
        public string? CreatedByEmployeeNumber { get; set; }

        // AuditLog
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
        public bool AuditLogShowComment { get; set; }
    }
}