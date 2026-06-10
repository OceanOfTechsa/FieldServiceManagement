using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.Organisation;
using FieldServiceManagement.ViewModels.SubscriptionPlan;

namespace FieldServiceManagement.ViewModels.User
{
    /// <summary>
    /// Aggregate view model for a user's full profile page.
    ///
    /// Composes the core <see cref="AppUserViewModel"/> with its related
    /// organisation, subscription plan, audit history, and any future
    /// extensions — without bloating a single flat class.
    ///
    /// Growth guide
    /// ────────────
    /// • Need work-order history?  Add  List&lt;WorkOrderSummaryViewModel&gt; WorkOrders
    /// • Need form submissions?    Add  List&lt;FormSubmissionSummaryViewModel&gt; FormSubmissions
    /// • Need permissions?         Add  List&lt;PermissionViewModel&gt; Permissions
    /// • Need devices / sessions?  Add  List&lt;UserSessionViewModel&gt; Sessions
    /// All additions stay isolated; no existing consumers break.
    /// </summary>
    public class AppUserProfileViewModel
    {
        // ─── Core user ────────────────────────────────────────────────────────────
        /// <summary>The user whose profile is being viewed.</summary>
        public AppUserViewModel User { get; set; } = new();

        // ─── Creator ──────────────────────────────────────────────────────────────
        /// <summary>
        /// The user who originally created this account.
        /// Null when the account was self-registered or the creator is deleted.
        /// </summary>
        public AppUserViewModel? CreatedBy { get; set; }

        // ─── Organisation ─────────────────────────────────────────────────────────
        /// <summary>The organisation this user belongs to.</summary>
        public OrganisationViewModel? Organisation { get; set; }

        // ─── Subscription plan ────────────────────────────────────────────────────
        /// <summary>
        /// The active subscription plan attached to the user's organisation.
        /// Null when no plan is assigned or the plan is inactive.
        /// </summary>
        public SubscriptionPlanViewModel? SubscriptionPlan { get; set; }

        // ─── Audit history ────────────────────────────────────────────────────────
        /// <summary>
        /// Chronological list of changes made directly to this user's profile
        /// (from [dbo].[UserProfileAudits]).
        /// </summary>
        public List<UserProfileAuditViewModel> ProfileAudits { get; set; } = new();

        /// <summary>
        /// Chronological list of form-submission audit events associated with
        /// this user (from [dbo].[FormSubmissionAudits]).
        /// Populated only when the caller opts-in (e.g. a detail page).
        /// </summary>
        //public List<FormSubmissionAuditViewModel> FormSubmissionAudits { get; set; } = new();

        // ─── Computed helpers ─────────────────────────────────────────────────────
        /// <summary>True when the organisation has a live subscription plan.</summary>
        public bool HasActivePlan =>
            SubscriptionPlan is { IsActive: true };

        /// <summary>True when the user belongs to a known organisation.</summary>
        public bool HasOrganisation =>
            Organisation is not null && Organisation.Id != Guid.Empty;

        /// <summary>Combined audit count across both audit tables.</summary>
        public int TotalAuditCount =>
            ProfileAudits.Count;

        public int numberOfUsers { get; set; }


        public UserInvitationViewModel? Invitation { get; set; }
    }
}