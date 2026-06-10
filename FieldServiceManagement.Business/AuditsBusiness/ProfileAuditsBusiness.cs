using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Data.DataModels.UserProfileAudit;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.Extensions;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.AuditsBusiness
{
    public class ProfileAuditsBusiness
    {
        public async Task<List<UserProfileAuditViewModel>> GetUserProfileAuditsByUserdAsync(Guid Id)
        {
            var audits = await new UserProfileAuditsRepository().GetAllUserProfileAuditsByUserIdAsync(Id);
            return ObjectMapper.Mapper.Map<List<UserProfileAuditViewModel>>(audits);
        }

        public async void LogProfileAuditAsync(UserInvitationViewModel model, AppUserProfileViewModel currentUser, AppUser userProfile)
        {
            new UserProfileAuditsRepository().LogProfileAuditAsync(new UserProfileAudit
            {
                OrganisationId = currentUser.User.OrganisationId,
                UserId = userProfile.Id,
                PerformByUserId = currentUser.User.Id,
                PerformedByName = currentUser.User.Name,
                Action = "ProfileCreated",
                Comment = $"User account created for {model.Email} with role {((UserRole)userProfile.UserRoleId!).GetDisplayName()} via invitation.",
                IpAddress = new LocalIPResolver().GetRoutedIPv4()!,
                IsDeleted = false,
                CreatedAt = DateTime.Now.SaDateTime()
            });
        }

        public async Task LogProfileUpdateAsync(AppUserViewModel before, AppUserViewModel after, AppUserViewModel performedBy)
        {
            var changes = PopulateProfileChanges(before, after);

            if (!changes.Any())
                return;

            var repo = new UserProfileAuditsRepository();
            var now = DateTime.Now.SaDateTime();
            var ip = new LocalIPResolver().GetRoutedIPv4()!;

            foreach (var change in changes)
            {
                repo.LogProfileAuditAsync(new UserProfileAudit
                {
                    OrganisationId = after.OrganisationId,
                    UserId = after.Id,
                    PerformByUserId = performedBy.Id,
                    PerformedByName = performedBy.Name,
                    Action = "ProfileUpdated",
                    Comment = $"{change.Field} was updated on profile {after.Name} {after.Surname}.",
                    FieldName = change.Field,
                    OldValue = change.OldValue,
                    NewValue = change.NewValue,
                    IpAddress = ip,
                    IsDeleted = false,
                    CreatedAt = now
                });
            }
        }

        private static List<ProfileChange> PopulateProfileChanges(AppUserViewModel before,AppUserViewModel after)
        {
            var changes = new List<ProfileChange>();

            AddChange(changes, "Name", before.Name, after.Name);
            AddChange(changes, "Employee Number", before.EmployeeNumber, after.EmployeeNumber);
            AddChange(changes, "Surname", before.Surname, after.Surname);
            AddChange(changes, "Phone", before.Phone, after.Phone);

            AddChange(changes, "UserRole",before.UserRoleId,after.UserRoleId,
                value => value.HasValue
                    ? ((UserRole)value.Value).GetDisplayName()
                    : null);

            AddChange(
                changes,
                "StatusId",
                before.StatusId,
                after.StatusId,
                value => value.HasValue
                    ? ((UserStatus)value.Value).ToString()
                    : null);

            AddChange(
                changes,
                "PreferredLanguageId",
                before.PreferredLanguageId,
                after.PreferredLanguageId,
                value => value?.ToString());

            AddChange(
                changes,
                "IsActive",
                before.IsActive,
                after.IsActive,
                value => value.ToString());

            return changes;
        }

        private static void AddChange<T>(
            ICollection<ProfileChange> changes,
            string field,
            T oldValue,
            T newValue,
            Func<T, string?>? formatter = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return;

            formatter ??= value => value?.ToString();

            changes.Add(new ProfileChange
            {
                Field = field,
                OldValue = formatter(oldValue),
                NewValue = formatter(newValue)
            });
        }

        private class ProfileChange
        {
            public string Field { get; set; } = string.Empty;
            public string? OldValue { get; set; }
            public string? NewValue { get; set; }
        }
    }
}