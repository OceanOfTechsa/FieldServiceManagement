using FieldServiceManagement.Business.CrewBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Crew;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
    [Route("Workforce/[controller]")]
    public class CrewController : Controller
    {

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            var OrgId = User.GetOrganisationIdOrThrow();
            var contacts = await new CrewBusiness().GetAllOrganisationCrews(OrgId);
            if (!contacts.Any())
                return View("~/Views/Shared/PageEmptyState.cshtml", PageEmptyStates.Crew);

            ViewBag.SelectedStatus = status;
            return View(contacts);
        }

        [Authorize(Roles = "SuperAdmin, Administrator,Dispatcher")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(new CreateCrewViewModel());
        }


        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateCrew(CreateCrewViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Workforce/Crew/Create.cshtml", model);

            var result = await new CrewBusiness().CreateCrew(model, User?.Identity?.Name!);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message!);
                return View("~/Views/Workforce/Crew/Create.cshtml",model);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpGet("Edit")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var OrgId = User.GetOrganisationIdOrThrow();
            var model = await new CrewBusiness().GetCrewByIdAsync(Id, OrgId);
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Crew" });
            return View(model);
        }


        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpGet("Info")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Info(Guid Id)
        {
            var OrgId = User.GetOrganisationIdOrThrow();
            var crew = await new CrewBusiness().GetCrewDetailsByIdAsync(Id,OrgId);
            if (crew.Id == Guid.Empty)
                return View("ResourceNotFound", new ResourceNotFoundViewModel{ResourceName = "Crew" });
            
            if (crew.OrganisationId != OrgId)
                return Forbid();

            return View(crew);
        }

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("AddMember")]
        public async Task<IActionResult> AddMember([FromBody] AddCrewMemberRequest request)
        {
            if (request == null || request.UserId == Guid.Empty || request.CrewId == Guid.Empty)
                return Json(new { success = false, errorMessage = "Invalid request. Please select a crew." });

            try
            {
                var organisationId = User.GetOrganisationIdOrThrow();
                var currentUser = await new UserBusiness().GetUserDetailsByUserNameAsync(User?.Identity?.Name!);

                if (currentUser == null)
                    return Json(new { success = false, errorMessage = "Unable to identify the current user." });

                var result = await new CrewBusiness().AddCrewMemberAsync(
                    request.CrewId,
                    request.UserId,
                    organisationId,
                    currentUser.Id,
                    request.IsLead
                );

                if (!result.Success)
                    return Json(new { success = false, errorMessage = result.Message });

                return Json(new { success = true, message = "User added to crew successfully." });
            }
            catch (Exception)
            {
                return Json(new { success = false, errorMessage = "An unexpected error occurred. Please try again." });
            }
        }

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("ChangeLead")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeLead([FromBody] ChangeCrewLeadRequest request)
        {
            if (request is null || !ModelState.IsValid)
                return Json(new { success = false, errorMessage = "Invalid request." });
            
            if (request.CrewId == Guid.Empty || request.UserId == Guid.Empty)
                return Json(new { success = false, errorMessage = "Please select a user." });
            

            var organisationId = User.GetOrganisationIdOrThrow();
            var modifiedBy = User.Identity?.Name
                ?? throw new InvalidOperationException("Unable to resolve acting user.");

            var result = await new CrewBusiness().ChangeLeadAsync(request.CrewId, request.UserId, organisationId, modifiedBy);

            return Json(new { success = result, errorMessage = result.Message  ?? "Something went wrong. Please try again."});
        }

        // -----------------------------------------------------------------
        // Edit crew details
        // -----------------------------------------------------------------
        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("Edit")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CrewFullProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                return Json(new
                {
                    success = false,
                    errorMessage = "Please fix the errors below.",
                    errors
                });
            }

            var organisationId = User.GetOrganisationIdOrThrow();
            var result = await new CrewBusiness().UpdateCrewAsync(model, organisationId, User.Identity?.Name!);

            return Json(new { success = result.Success, errorMessage = result.Message });
        }

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("RemoveMember")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember([FromBody] RemoveCrewMemberRequest request)
        {
            if (request is null || !ModelState.IsValid)
                return Json(new { success = false, errorMessage = "Invalid request." });

            if (request.CrewId == Guid.Empty || request.UserId == Guid.Empty)
                return Json(new { success = false, errorMessage = "Invalid crew or user." });
            
            var organisationId = User.GetOrganisationIdOrThrow();
            var result = await new CrewBusiness().RemoveMemberAsync(
                request.CrewId, request.UserId, organisationId, User.Identity?.Name!);

            return Json(new { success = result.Success, errorMessage = result.Message });
        }

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteCrewRequest request)
        {
            if (request is null || !ModelState.IsValid)
                return Json(new { success = false, errorMessage = "Invalid request." });

            if (request.CrewId == Guid.Empty)
                return Json(new { success = false, errorMessage = "Invalid crew." });
            
            var organisationId = User.GetOrganisationIdOrThrow();
            var result = await new CrewBusiness().DeleteCrewAsync(request.CrewId, organisationId);
            return Json(new { success = result.Success, errorMessage = result.Message ?? "Something went wrong, please try again." });
        }
    }
}