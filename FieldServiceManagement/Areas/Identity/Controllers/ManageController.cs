using FieldServiceManagement.Business.LanguageBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.ViewModels.Shared;
using FieldServiceManagement.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FieldServiceManagement.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("Identity/Account/Manage")]
    [Authorize]
    public class ManageController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ManageController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _env = env;
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Personal")]
        public async Task<IActionResult> Personal()
        {
            var model = await new UserBusiness().GetUserDetailsByUserNameAsync(User?.Identity?.Name!);
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel{ ResourceName = "Profile" });

            var identityUser = await _userManager.GetUserAsync(User!);
            model.TwoFactorEnabled = identityUser?.TwoFactorEnabled;
            BuildLanguagesList(model);
            return View(model);
        }


        private void BuildLanguagesList(AppUserViewModel model)
        {
            var langs = new LanguageBusiness().GetLanguages();
            ViewBag.Languages = new SelectList(langs, "Id", "Name", model.PreferredLanguageId);
        }
    }
}
