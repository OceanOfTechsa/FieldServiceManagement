namespace FieldServiceManagement.ViewModels.Audit
{
    /// <summary>
    /// A single row from [dbo].[UserProfileAudits].
    /// </summary>
    public class UserProfileAuditViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public Guid PerformByUserId { get; set; }

        /// <summary>
        /// Display name of the user who performed the action.
        /// </summary>
        public string? PerformedByName { get; set; }
        public string? PerformedByAvatarUrl { get; set; }

        /// <summary>
        /// Action string from the DB (e.g. "ProfileUpdated", "RoleChanged")
        /// </summary>
        public string Action { get; set; } = string.Empty;

        public string? Comment { get; set; }

        // ─── NEW: Change Tracking Fields ─────────────────────────────────────
        /// <summary>
        /// The specific field that was changed (e.g. "PhoneNumber", "Email", "FullName")
        /// </summary>
        public string? FieldName { get; set; }

        /// <summary>
        /// Previous value before the change
        /// </summary>
        public string? OldValue { get; set; }

        /// <summary>
        /// New value after the change
        /// </summary>
        public string? NewValue { get; set; }

        public string? IpAddress { get; set; }
        public bool IsDeleted { get; set; }
    }
}
