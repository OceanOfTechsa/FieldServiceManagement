using Microsoft.AspNetCore.Mvc;
using FieldServiceManagement.Models;
using FieldServiceManagement.Helpers;
using Microsoft.AspNetCore.Authorization;
using FieldServiceManagement.ViewModels.Shared;
using FieldServiceManagement.ViewModels.Equipment;
using FieldServiceManagement.Business.EquipementBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;

namespace FieldServiceManagement.Controllers.WorkForce
{
    [Authorize(Roles = "SuperAdmin, Administrator")]
    public class EquipmentController : Controller
    {
        public async Task<IActionResult> Index(string? status)
        {
            var OrgId = User.GetOrganisationIdOrThrow();
            var equipments = await new EquipmentBusiness().GetEquipmentsByOrganisationId(OrgId);
            if(!equipments.Any())
                return View("~/Views/Shared/PageEmptyState.cshtml", PageEmptyStates.Equipment);

            ViewBag.SelectedStatus = status;
            return View(equipments);
        }

        public IActionResult Create()
        {
            var model = new CreateEquipmentViewModel()
            {
                CreatedByEmail = User.Identity?.Name!,
                OrganisationId = User.GetOrganisationIdOrThrow()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEquipmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Workforce/Equipment/Create.cshtml", model);

            var result = await new EquipmentBusiness().CreateEquipment(model);
            if(!result.HasValue)
                return View(model);

            return RedirectToAction("Info", new { Id = UrlEncryptionBusiness.EncryptParam(result?.ToString()!)});
        }

        [MVCDecryptFilter]
        public async Task<IActionResult> Info(int? page, Guid Id)
        {
            var model = await new EquipmentBusiness().GetEquipmentDetailsAsync(Id, User?.Identity?.Name!);

            if(model?.Equipment is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Equipment" });

            if (model.Equipment.OrganisationId != User?.GetOrganisationIdOrThrow())
                return Forbid();

            return View(model);
        }

        [MVCDecryptFilter]
        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await new EquipmentBusiness().GetEquipmentByIdAsync(Id);
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Equipment" });

            if (model.OrganisationId != User.GetOrganisationIdOrThrow())
                return Forbid();

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        public async Task<IActionResult> Edit(EquipmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Workforce/Equipment/Edit.cshtml", model);

            var result = await new EquipmentBusiness().UpdateEquipmentAsync(model, User?.Identity?.Name!);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Failed to update equipment.");
                return View("~/Views/Workforce/Equipment/Edit.cshtml", model);
            }

            return RedirectToAction("Info", new
            {
                Id = UrlEncryptionBusiness.EncryptParam(model.Id.ToString()!)
            });
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        public async Task<IActionResult> Delete([FromBody] DeleteEquipmentRequest request)
        {
            if (request is null || !ModelState.IsValid)
                return Json(new { success = false, errorMessage = "Invalid request." });

            if (request.EquipmentId == Guid.Empty)
                return Json(new { success = false, errorMessage = "Invalid crew." });

            return View();
        }
    }
}
