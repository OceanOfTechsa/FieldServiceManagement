using FieldServiceManagement.Business;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data.DataModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    [Route("Identity/Account/Manage/Settings")]
    public class SettingsController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public SettingsController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _env = env;
        }

        [Authorize(Roles = "Administrator, SuperAdmin")]
        [HttpGet("Organisation")]
        public async Task<IActionResult> Organisation()
        {
            var user = await new UserBusiness().GetUserDetailsByUserNameAsync(User.Identity.Name);
            var organisation = await new OrganisationBusiness().GetOrganisationById(user.OrganisationId);

            return View(organisation);
        }
    }
}
