
using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.User
{
    public class UserProfileDetailsViewModel : BaseGuidPrimaryKeyViewModel
    {
        // ─── Core user ────────────────────────────────────────────────
        public Guid OrganisationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int? UserRoleId { get; set; }
        public int? PreferredLanguageId { get; set; }
        public bool IsActive { get; set; }
        public int? StatusId { get; set; }
        public bool IsOwner { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public string? EmployeeNumber { get; set; }

        // ─── Created by ───────────────────────────────────────────────
        public Guid? CB_Id { get; set; }
        public string? CB_Name { get; set; }
        public string? CB_Surname { get; set; }
        public string? CB_Email { get; set; }
        public string? CB_AvatarUrl { get; set; }
        public bool? CB_IsActive { get; set; }

        // ─── Organisation ─────────────────────────────────────────────
        public Guid? Org_Id { get; set; }
        public string? Org_Name { get; set; }
        public string? Org_AvatarUrl { get; set; }
        public int? Org_IndustryId { get; set; }
        public int? Org_CountryId { get; set; }
        public int? Org_StateId { get; set; }
        public int? Org_CurrencyId { get; set; }
        public int? Org_TimezoneId { get; set; }
        public int? Org_LanguageId { get; set; }
        public int? Org_PlanId { get; set; }
        public bool? Org_IsActive { get; set; }
        public bool? Org_IsDeleted { get; set; }
        public DateTime? Org_CreatedAt { get; set; }
        public DateTime? Org_UpdatedAt { get; set; }
        public Guid? Org_CreatedById { get; set; }
        public Guid? Org_UpdatedById { get; set; }

        // ─── Organisation subscription ────────────────────────────────
        public Guid? Sub_Id { get; set; }
        public int? Sub_PlanId { get; set; }
        public string? Sub_BillingCycle { get; set; }
        public DateTime? Sub_StartDate { get; set; }
        public DateTime? Sub_EndDate { get; set; }
        public bool? Sub_IsActive { get; set; }
        public DateTime? Sub_CancelledAt { get; set; }
        public string? Sub_CancellationNote { get; set; }
        public DateTime? Sub_CreatedAt { get; set; }

        // ─── Subscription plan ────────────────────────────────────────
        public int? Plan_Id { get; set; }
        public string? Plan_Name { get; set; }
        public string? Plan_Description { get; set; }
        public int? Plan_MaxUsers { get; set; }
        public int? Plan_MaxWorkOrders { get; set; }
        public int? Plan_MaxForms { get; set; }
        public int? Plan_MaxStorageMb { get; set; }
        public decimal? Plan_MonthlyPrice { get; set; }
        public decimal? Plan_AnnualPrice { get; set; }
        public string? Plan_CurrencyCode { get; set; }
        public bool? Plan_IsActive { get; set; }
        public DateTime? Plan_CreatedAt { get; set; }

        // ─── Invitation ───────────────────────────────────────────────
        public int? Inv_Id { get; set; }
        public string? Inv_Name { get; set; }
        public string? Inv_Surname { get; set; }
        public string? Inv_Email { get; set; }
        public int? Inv_UserType { get; set; }
        public Guid? Inv_CreatedBy { get; set; }
        public DateTime? Inv_CreatedAt { get; set; }
        public DateTime? Inv_ExpiresAt { get; set; }

        // ─── Counts ───────────────────────────────────────────────────
        public int NumberOfUsers { get; set; }

        // ─── Audit log (repeated per row) ────────────────────────────
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
