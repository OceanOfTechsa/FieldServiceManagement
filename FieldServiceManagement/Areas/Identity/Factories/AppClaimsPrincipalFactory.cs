using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Enum;
using FieldServiceManagement.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FieldServiceManagement.Areas.Identity.Factories
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var profile = await GetUserProfileByUserName(user.Email!);

            if (profile == null)
                throw new InvalidLoginException(InvalidLoginReason.ProfileNotFound);

            if (!profile.IsActive || profile.StatusId == (int)UserStatus.Deleted)
                throw new InvalidLoginException(InvalidLoginReason.AccountInactive);

            string? role = ResolveRole(profile);
            if (string.IsNullOrWhiteSpace(role))
                throw new InvalidLoginException(InvalidLoginReason.InvalidRole);

            var existingRoleClaim = identity.FindFirst(ClaimTypes.Role);
            if (existingRoleClaim != null)
                identity.RemoveClaim(existingRoleClaim);

            identity.AddClaim(new Claim(ClaimTypes.Role, role));

            if (user.OrganisationId.HasValue)
                identity.AddClaim(new Claim("OrganisationId", user.OrganisationId.Value.ToString()));

            return identity;
        }

        private static async Task<AppUserViewModel> GetUserProfileByUserName(string email)
        {
            if(string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            return await new UserBusiness().GetUserDetailsByUserNameAsync(email);
        }

        private static string? ResolveRole(AppUserViewModel Profile)
        {
            if (Profile.UserRoleId != null && System.Enum.IsDefined(typeof(UserRole), Profile.UserRoleId.Value))
            {
                var roleEnum = (UserRole)Profile.UserRoleId.Value;
                return roleEnum.GetDisplayName();
            }
            return Profile.UserRoleId?.ToString();
        }
    }
}
