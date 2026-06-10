using FieldServiceManagement.Areas.Identity.Models;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class OnboardingController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public OnboardingController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet, MVCDecryptFilter]
        [Route("Identity/Onboarding")]
        public async Task<ActionResult> Index(string token)
        {
            if(string.IsNullOrEmpty(token))
                return RedirectToAction("AccessDenied", "Account");
            var user = await _userManager.FindByEmailAsync(token);
            if (user == null)
                return RedirectToAction("AccessDenied", "Account");

            return View(new OnboardingViewModel{Email = token});
        }
    
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(OnboardingViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Onboarding", new { area = "Identity",  email = model.Email });

            var identityUserId = (await _userManager.FindByEmailAsync(model.Email))?.Id;
            if (identityUserId == null)
                return RedirectToAction("Login", "Account");

            var (userId, organisationId) = await new UserBusiness().CompleteOnboardingAsync(model, identityUserId);

            return RedirectToAction("Index", "Home", new { area = "" });
        }


        [Authorize]
        public ActionResult Organisation()
        {

            return View();
        }


        [Authorize]
        public ActionResult Timezone()
        {

            return View();
        }


        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        private string GetBrowser()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
