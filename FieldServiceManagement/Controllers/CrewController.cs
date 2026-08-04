using FieldServiceManagement.Business.CrewBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Crew;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

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

        public class AddCrewMemberRequest
        {
            public Guid UserId { get; set; }
            public Guid CrewId { get; set; }
            public bool IsLead { get; set; }
        }
    }
}
