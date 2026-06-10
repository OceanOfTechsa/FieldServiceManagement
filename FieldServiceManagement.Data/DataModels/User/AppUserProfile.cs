namespace FieldServiceManagement.Data.DataModels.User
{
    public class AppUserProfile
    {
        // ─── User ────────────────────────────────────────────────────────────────
        public Guid UserId { get; set; }
        public Guid OrganisationId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;

        public string FullName => $"{Name} {Surname}".Trim();

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string? UserAvatarUrl { get; set; }
        public int? UserRoleId { get; set; }
        public int? PreferredLanguageId { get; set; }
        public bool UserIsActive { get; set; }
        public int? UserStatusId { get; set; }
        public bool IsOwner { get; set; }
        public bool UserIsDeleted { get; set; }

        public DateTime UserCreatedAt { get; set; }
        public DateTime? UserUpdatedAt { get; set; }

        public Guid UserCreatedById { get; set; }
        public Guid? UserUpdatedById { get; set; }

        // ─── Created By User (NEW) ──────────────────────────────────────────────
        public Guid? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedBySurname { get; set; }
        public string? CreatedByEmail { get; set; }
        public string? CreatedByAvatarUrl { get; set; }

        public string? CreatedByFullName =>
            string.IsNullOrWhiteSpace(CreatedByName) && string.IsNullOrWhiteSpace(CreatedBySurname)
                ? null
                : $"{CreatedByName} {CreatedBySurname}".Trim();

        // ─── Organisation ───────────────────────────────────────────────────────
        //public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string? OrganisationAvatarUrl { get; set; }

        public int? IndustryId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CurrencyId { get; set; }
        public int? TimezoneId { get; set; }
        public int? LanguageId { get; set; }
        public int? PlanId { get; set; }

        public bool OrganisationIsActive { get; set; }
        public bool OrganisationIsDeleted { get; set; }

        public DateTime OrganisationCreatedAt { get; set; }
        public DateTime? OrganisationUpdatedAt { get; set; }

        public Guid OrganisationCreatedById { get; set; }
        public Guid? OrganisationUpdatedById { get; set; }

        // ─── Subscription Plan ──────────────────────────────────────────────────
        public int? SubscriptionPlanId { get; set; }
        public string? PlanName { get; set; }
        public string? PlanDescription { get; set; }

        public int? MaxUsers { get; set; }
        public int? MaxWorkOrders { get; set; }
        public int? MaxForms { get; set; }
        public int? MaxStorageMb { get; set; }

        public decimal? MonthlyPrice { get; set; }
        public decimal? AnnualPrice { get; set; }
        public string? CurrencyCode { get; set; }

        public bool PlanIsActive { get; set; }
        public DateTime? PlanCreatedAt { get; set; }
    }
}