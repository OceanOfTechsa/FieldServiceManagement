using FieldServiceManagement.Business.AnnouncementBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Announcement;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize]
    public class AnnouncementsController : Controller
    {
        #region User Routes

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await new AnnouncementBusiness()
                .GetAnnouncementsForUserAsync(User.Identity?.Name!);

            return View(result.Announcements.Any()
                ? result
                : new AnnouncementListViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetAnnouncementsPartial()
        {
            var result = await new AnnouncementBusiness()
                .GetAnnouncementsForUserAsync(User.Identity?.Name!);

            return PartialView(
                "~/Views/Components/Partials/_Announcements.cshtml",
                result);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsSeen(Guid id)
        {
            var result = await new AnnouncementBusiness()
                .MarkAnnouncementAsSeenAsync(id, User.Identity?.Name!);

            if (!result)
            {
                return BadRequest("Failed to mark announcement as seen.");
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetUnseenCount()
        {
            var count = await new AnnouncementBusiness()
                .GetUnseenAnnouncementCountAsync(User.Identity?.Name!);

            return Ok(new { count });
        }

        #endregion

        #region Super Admin Routes

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("/Admin/Announcements")]
        public async Task<IActionResult> IndexAdmin(string? status)
        {
            var result = await new AnnouncementBusiness().GetAllAnnouncementsForAdminAsync();
            ViewBag.SelectedStatus = status;
            return View("~/Views/Admin/Announcements/Index.cshtml", result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("/Admin/Announcements/Create")]
        public IActionResult Create()
        {
            return View("~/Views/Admin/Announcements/Create.cshtml", new CreateAnnouncementViewModel());
        }


        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("/Admin/Announcements/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnnouncementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/Announcements/Create.cshtml", model);
            }

            var newId = await new AnnouncementBusiness().CreateAnnouncementAsync(model, User.Identity?.Name!);

            if (newId == Guid.Empty)
            {
                ModelState.AddModelError("", "Failed to create announcement.");
                return View("~/Views/Admin/Announcements/Create.cshtml", model);
            }

            return RedirectToAction(nameof(IndexAdmin));
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("/Admin/Announcements/Edit")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var repo = new AnnouncementBusiness();
            var announcement = await repo.GetAnnouncementById(Id);
            if(announcement is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Announcement", Title = "Announcement Not Found" });

            var model = await repo.PopulateEditAnnouncementModel(announcement);
            return View("~/Views/Admin/Announcements/Edit.cshtml", model);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("/Admin/Announcements/Edit")]
        public async Task<IActionResult> Edit(EditAnnouncementViewModel model)
        {
            if(!ModelState.IsValid)
                return View("~/Views/Admin/Announcements/Edit.cshtml", model);

            var success = await new AnnouncementBusiness().UpdateAnnouncementAsync(model, User?.Identity?.Name!);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Failed to update the announcement. Please try again.");
                return View(model);
            }
            return Redirect($"/Admin/Announcements/Info?Id={UrlEncryptionBusiness.EncryptParam(model.Id.ToString())}");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("/Admin/Announcements/Info")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Info(Guid Id)
        {
            var announcement = await new AnnouncementBusiness().GetAnnouncementDetailsByIdAsync(Id);
            if(announcement is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Announcement", Title = "Announcement Not Found" });
            return View("~/Views/Admin/Announcements/Info.cshtml", announcement);
        }

        [HttpPost("/Admin/Announcements/Archive")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Archive([FromBody] AnnouncementRequest request)
        {
            if (request.Id == Guid.Empty)
                return BadRequest();

            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var success = await new AnnouncementBusiness().ArchiveAnnouncementByIdAsync(request.Id, email);
            return success ? Ok() : StatusCode(500);
        }

        [HttpPost("/Admin/Announcements/Delete")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete([FromBody] AnnouncementRequest request)
        {
            if (request.Id == Guid.Empty)
                return BadRequest();

            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var success = await new AnnouncementBusiness().DeleteAnnouncementByIdAsync(request.Id, email);
            return success ? Ok() : StatusCode(500);
        }

        public record AnnouncementRequest(Guid Id);
        #endregion
    }
}