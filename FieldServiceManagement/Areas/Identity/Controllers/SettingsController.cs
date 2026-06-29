using FieldServiceManagement.Business;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.ViewModels.Shared;
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
            var model = await new OrganisationBusiness().GetOrganisationDeatailsByIdAsync(User.GetOrganisationIdOrThrow());
            if(model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Organisation", ReturnUrl = "/Identity/Account/Manage/Settings/Organisation" });

            return View(model);
        }

        [Authorize(Roles = "Administrator, SuperAdmin")]
        [HttpGet("Addresses")]
        public async Task<IActionResult> Addresses()
        {
            //var model = await new OrganisationBusiness().GetOrganisationById(User.GetOrganisationIdOrThrow());
            //if (model is null)
            //    return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Organisation", ReturnUrl = "/Identity/Account/Manage/Settings/Organisation" });

            return View();
        }
    }
}
