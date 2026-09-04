using AutoMapper;
using FieldServiceManagement.Business.EquipementBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Equipment; 
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var model = await new EquipmentBusiness().GetEquipmentDetailsAsync(Id, User?.Identity?.Name!);

            if(model?.Equipment is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Equipment" });

            if (model.Equipment.OrganisationId != User?.GetOrganisationIdOrThrow())
                return Forbid();

            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpGet("Edit")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await new EquipmentBusiness().GetEquipmentByIdAsync(Id);
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Equipment" });

            if (model.OrganisationId != User.GetOrganisationIdOrThrow())
                return Forbid();

            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
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

        [Authorize(Roles = "SuperAdmin, Administrator, Dispatcher")]
        [HttpPost("Delete")]
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
