using FieldServiceManagement.Areas.Identity.Models;
using FieldServiceManagement.Business.AuditsBusiness;
using FieldServiceManagement.Business.AuthorisationBusiness;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.NotificationBusiness.IdentityNotifications;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Extensions;
using FieldServiceManagement.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace FieldServiceManagement.Business.UserBusiness
{
    public class UserBusiness
    {
        public async Task<(Guid UserId, Guid OrganisationId)> CompleteOnboardingAsync(OnboardingViewModel Model, string IdentityUserId)
        {
            return await new UserRepository().CreateUserProfileAndOrganisationAsync(Model,IdentityUserId, new LocalIPResolver().GetRoutedIPv4());
        }
        public async Task<AppUserViewModel> GetUserDetailsByUserNameAsync(string Username)
        {
            var user = new UserRepository().GetByUserName(Username);
            return ObjectMapper.Mapper.Map<AppUserViewModel>(user);
        }

        public async Task<AppUserViewModel> GetUserDetailsByIdAsync(Guid Id)
        {
            var user = new UserRepository().GetById(Id);
            return ObjectMapper.Mapper.Map<AppUserViewModel>(user);
        }

        public async Task<IEnumerable<AppUserProfileViewModel>> GetAllUsersByEmailAsync(string Email)
        {
            var users = await new UserRepository().GetAllUsersByEmailAsync(Email);
            var result = new List<AppUserProfileViewModel>();
            foreach (var user in users)
            {
                var userDetails = await GetAllUserDetailsByUsernameAsync(user.Email);
                result.Add(userDetails);
            }
            return result;
        }

        public Guid GetUserOrganisationIdByUserEmail(string Email)
        {
            return new UserRepository().GetUserOrganisationIdByUserEmail(Email);
        }

        public async Task<BusinessResult> SendUserInvitationAsync(UserInvitationViewModel Model, string Email, UserManager<ApplicationUser> UserManager)
        {
            var existingUser = new UserRepository().GetByUserName(Model.Email);
            if (existingUser != null)
                return BusinessResult.Fail("A user account already exists with this email address.");

            var currentUser = await new UserBusiness().GetAllUserDetailsByUsernameAsync(Email!);
            var result = await new SubscriptionPlanBusiness().ApplySubscriptionPlanRules(currentUser, SubscriptionRuleContext.AddUser);
            if (!result.Success) return result;

            try
            {
                var tempPass = GenerateTemporaryPassword();
                var registerResult = await new AuthBusiness(UserManager).RegisterUserAsync(Model.Email, tempPass);
                if (!registerResult.Succeeded)
                    return BusinessResult.Fail(registerResult.Errors.FirstOrDefault()?.Description ?? "Failed to register user.");

                var userInvitationBusiness = new UserInvitationBusiness();
                var existingInvitation = userInvitationBusiness.CheckIfUserInvitationExistsByEmail(Model.Email, currentUser.User.OrganisationId);
                if (existingInvitation != null)
                    return BusinessResult.Fail("An invitation has already been sent to this email address for your organisation.");
                userInvitationBusiness.CreateUserInvitationAsync(Model, currentUser);

                var createdUser = await InitiateUserProfile(Model, currentUser, UserManager);

                new ProfileAuditsBusiness().LogProfileAuditAsync(Model, currentUser, createdUser!);
                new InvitationNotification(currentUser, Model, tempPass).SendNotificationWithoutQueue();
                return BusinessResult.Ok();
            }
            catch (Exception ex)
            {
                return BusinessResult.Fail("An error occurred while sending the invitation.");
            }
        }

        public async Task<AppUserProfileViewModel> GetAllUserDetailsByUsernameAsync(string Username)
            => await BuildUserProfileAsync(ByEmail: Username, ById: null);

        public async Task<AppUserProfileViewModel> GetAllUserDetailsByIdAsync(Guid UserId)
            => await BuildUserProfileAsync(ByEmail: null, ById: UserId);
       
        public async Task<Guid?> UpdateUserAsync(AppUserViewModel Model, string Email)
        {
            var dbModel = ObjectMapper.Mapper.Map<AppUser>(Model);
            dbModel.AvatarUrl = AvatarHelper.GetAvatar(Model.Name, Model.Surname);
            return await new UserRepository().UpdateUserProfileAsync(dbModel, Email, new LocalIPResolver().GetRoutedIPv4()!);
        }

        // <summary>
        /// Unified entry-point — exactly one of <paramref name="ByEmail"/> or
        /// <paramref name="ById"/> must be supplied; the other should be null.
        /// </summary>
        private async Task<AppUserProfileViewModel> BuildUserProfileAsync(string? ByEmail,Guid? ById)
        {
            AppUserViewModel user = ById.HasValue
                ? await GetUserDetailsByIdAsync(ById.Value)
                : await GetUserDetailsByUserNameAsync(ByEmail!);

            if (user is null)
                throw new KeyNotFoundException(ById.HasValue
                        ? $"No user found with Id '{ById}'"
                        : $"No user found with username '{ByEmail}'"
                );

            var Model = new AppUserProfileViewModel { User = user };
            Model.Organisation = await new OrganisationBusiness().GetOrganisationById(Model.User.OrganisationId);
            
            bool canSeePlan = Model.Organisation?.CreatedById == Model.User.Id || Model.User.UserRoleId == (int)UserRole.SuperAdmin || Model.User.UserRoleId == (int)UserRole.Administrator;
            if (canSeePlan)
            {
                Model.OrgSubscription = await new OrganisationSubscriptionBusiness().GetOrganisationSubscriptionByOrgIdAsync(Model!.Organisation!.Id!);
                Model.SubscriptionPlan = await new SubscriptionPlanBusiness().GetSubscriptionPlanByIdAsync(Model.OrgSubscription.PlanId);
            }
            if (user.StatusId == (int)UserStatus.Invited)
            {
                Model.Invitation = await new UserInvitationBusiness().GetUserInvitationByEmail(user.Email);
            }
            var entiityName = "Profile";
            Model.ProfileAudits = await new AuditLogBusiness().GetByEntityAsync(entiityName, Model.User.Id, Model.User.OrganisationId);
            Model.CreatedBy = await GetUserDetailsByIdAsync(Model.User.CreatedById);
            return Model;
        }

        public async Task<List<string>> GetEmailsByRoleIdsAsync(List<int> RoleIds)
        {
            if (RoleIds == null || !RoleIds.Any())
                return new List<string>();

            return await new UserRepository().GetEmailsByRoleIdsAsync(RoleIds);
        }

        public async Task<List<UserSearchResultViewModel>> SearchUsersAsync(string term, Guid organisationId)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<UserSearchResultViewModel>();

            if (!term.Equals("all", StringComparison.OrdinalIgnoreCase) && term.Length < 2)
                return new List<UserSearchResultViewModel>();
            
            var results = await new UserRepository().SearchUsersAsync(term.Trim(), organisationId);

            return ObjectMapper.Mapper.Map<List<UserSearchResultViewModel>>(results);
        }

        #region PRIVATE METHODS
        public string GenerateTemporaryPassword(int Length = 12)
        {
            if (Length < 8)
                throw new ArgumentException("Password length must be at least 8 characters.");

            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string symbols = "!@#$%^&*()-_=+[]{};:,.<>?";

            string all = lower + upper + numbers + symbols;

            var password = new char[Length];
            var rng = RandomNumberGenerator.Create();

            password[0] = lower[GetRandomIndex(rng, lower.Length)];
            password[1] = upper[GetRandomIndex(rng, upper.Length)];
            password[2] = numbers[GetRandomIndex(rng, numbers.Length)];
            password[3] = symbols[GetRandomIndex(rng, symbols.Length)];

            for (int i = 4; i < Length; i++)
            {
                password[i] = all[GetRandomIndex(rng, all.Length)];
            }
            return new string(password.OrderBy(_ => GetRandomInt(rng)).ToArray());
        }

        private int GetRandomIndex(RandomNumberGenerator Rng, int Max)
        {
            return GetRandomInt(Rng) % Max;
        }

        private int GetRandomInt(RandomNumberGenerator Rng)
        {
            var bytes = new byte[4];
            Rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0) & int.MaxValue;
        }

        private static async Task<AppUser?> InitiateUserProfile(UserInvitationViewModel Model, AppUserProfileViewModel CurrentUser, UserManager<ApplicationUser> UserManager)
        {
            var user = await UserManager.FindByEmailAsync(Model.Email);
            if(user == null)
                return null;

            AppUser? createdUser = await new UserRepository().InsertAppUser(new AppUser
            {
                OrganisationId = CurrentUser.User.OrganisationId,
                Name = Model.Name,
                Surname = Model.Surname,
                Email = Model.Email,
                Phone = string.Empty,
                AvatarUrl = string.Empty,
                UserRoleId = Model.UserType,
                IsActive = true,
                StatusId = (int)UserStatus.Invited,
                IsOwner = false,
                UserId = Guid.Parse(user?.Id ?? Guid.Empty.ToString()),
                IsDeleted = false,
                CreatedAt = DateTime.Now.SaDateTime(),
                UpdatedAt = DateTime.Now.SaDateTime(),
                CreatedById = CurrentUser.User.Id,
                UpdatedById = CurrentUser.User.Id,
                EmployeeNumber = Model.EmployeeNumber,
                SalutationId = Model.SalutationId
            });

            return createdUser;
        }
        #endregion
    }
}
