using FieldServiceManagement.Business.EquipementBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Equipment; 
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Administrator")]
    [Route("Workforce/[controller]")]
    public class EquipmentController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            var OrgId = User.GetOrganisationIdOrThrow();
            var equipments = await new EquipmentBusiness().GetEquipmentsByOrganisationId(OrgId);
            if(!equipments.Any())
                return View("~/Views/Shared/PageEmptyState.cshtml", PageEmptyStates.Equipment);

            ViewBag.SelectedStatus = status;
            return View(equipments);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var model = new CreateEquipmentViewModel()
            {
                CreatedByEmail = User.Identity?.Name!,
                OrganisationId = User.GetOrganisationIdOrThrow()
            };
            return View(model);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateEquipmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Workforce/Equipment/Create.cshtml", model);

            var result = await new EquipmentBusiness().CreateEquipment(model);
            if(!result.HasValue)
                return View(model);

            return RedirectToAction("Info", new { Id = UrlEncryptionBusiness.EncryptParam(result?.ToString()!)});
        }

        [HttpGet("Info")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Info(int? page, Guid Id)
        {
            var model = await new EquipmentBusiness().GetEquipmentDetailsAsync(Id);
            if(!model.Any())
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Equipment" });
            return View(model.ToPagedList(page ?? 1, 10));
        }
    }
}
