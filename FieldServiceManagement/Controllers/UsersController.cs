using FieldServiceManagement.Business.LanguageBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Shared;
using FieldServiceManagement.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Administrator")]
    [Route("Workforce/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string? status, string? role)
        {
            var users = await new UserBusiness().GetAllUsersByEmailAsync(User?.Identity?.Name!);
            role ??= "All";

            ViewBag.SelectedStatus = status;
            ViewBag.SelectedRole = role;

            return View(users);
        }

        [HttpGet("Info")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Info(Guid Id)
        {
            var profile = await new UserBusiness().GetAllUserDetailsByIdAsync(Id);
            if (profile is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "User" });

            if (profile.User.OrganisationId != User.GetOrganisationId())
                return Forbid();

            return View(profile);
        }

        [HttpGet("Invite")]
        [Authorize(Roles = "SuperAdmin, Administrator")]
        public IActionResult Invite()
        {
            var model = new UserInvitationViewModel();
            return View(model);
        }

        [HttpPost("Invite")]
        [Authorize(Roles = "SuperAdmin, Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(UserInvitationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await new UserBusiness().SendUserInvitationAsync(model, User?.Identity?.Name!, _userManager);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message!);
                return View(model);
            }
                
            return RedirectToAction("Index");
        }


        [HttpGet("Edit")]
        [Authorize(Roles = "SuperAdmin, Administrator")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await new UserBusiness().GetAllUserDetailsByIdAsync(Id);
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "User" });

            BuildLanguagesList(model);
            return View(model);
        }

        [HttpPost("Edit")]
        [Authorize(Roles = "SuperAdmin, Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUserProfileViewModel model)
        {
            var keysToRemove = ModelState.Keys.Where(k => !k.StartsWith(nameof(AppUserProfileViewModel.User))).ToList();
            foreach (var key in keysToRemove)
                ModelState.Remove(key);

            if (!ModelState.IsValid)
            {
                BuildLanguagesList(model);
                return View(model);
            }
        
            await new UserBusiness().UpdateUserAsync(model.User, User?.Identity?.Name!);
            return RedirectToAction("Info", "Users", new { Id = UrlEncryptionBusiness.EncryptParam(model.User.Id.ToString()) });
        }

        private void BuildLanguagesList(AppUserProfileViewModel model)
        {
            model.Languages = new LanguageBusiness().GetLanguages();
        }

        [Authorize(Roles = "SuperAdmin, Administrator")]
        [HttpPost("SendInvitationReminder")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> SendInvitationReminder([FromBody] ReminderRequest request)
        {
            if (request?.Email is null)
                return Json(new { result = new { success = false, message = "Email is required." } });


            var result = await new UserInvitationBusiness().SendInvitationReminder(request.Email);
            return Json(new { result });
        }


        public class ReminderRequest
        {
            public string Email { get; set; }
        }
    }
}
