using FieldServiceManagement.Data.DataModels.User;
using Microsoft.AspNetCore.Identity;

namespace FieldServiceManagement.Business.AuthorisationBusiness
{
    public class AuthBusiness
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthBusiness(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
            };

            return await _userManager.CreateAsync(user, password);
        }
    }
}