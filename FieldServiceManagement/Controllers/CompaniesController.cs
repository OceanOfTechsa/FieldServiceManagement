using FieldServiceManagement.Business.AddressBusiness;
using FieldServiceManagement.Business.CompanyBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Company;
using FieldServiceManagement.ViewModels.Interfaces;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Route("Customers/[controller]")]
    [Authorize]
    public class CompaniesController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            var companies = await new CompanyBusiness().GetCompaniesByUserEmailAsync(User?.Identity?.Name!);
            if (!companies.Any())
                return View("~/Views/Shared/PageEmptyState.cshtml", PageEmptyStates.Companies);
            
            ViewBag.SelectedStatus = status;
            return View(companies);
        }

        [HttpGet("Info")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Info(int? page, Guid Id)
        {
            var Model = await new CompanyBusiness().GetCompanyFullDetailsByIdAsync(Id, User?.Identity?.Name!);
            if (Model.Company is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Company" });

            if (Model.Company.OrganisationId != User?.GetOrganisationIdOrThrow())
                return Forbid();

            Model.OrganisationAddresses = await new AddressBusiness().GetAvailableAddressesForCompanyAsync(User?.Identity?.Name!, Model.Company.Id);
            return View(Model);
        }

        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        [HttpGet("Create")]
        public async Task<IActionResult> Create(string? returnUrl)
        {
            var model = new CreateCompanyViewModel{ReturnUrl = returnUrl};
            await PopulateOrganisationAddresses(model);
            return View(model);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyViewModel model)
        {

            if (!ModelState.IsValid)
            {
                await PopulateOrganisationAddresses(model);
                return View(model);
            }
            var companyId = await new CompanyBusiness().CreateCompanyAsync(model, User?.Identity?.Name!);
            return RedirectToAction("Info", new { Id = UrlEncryptionBusiness.EncryptParam(companyId.ToString()!) });
        }

        [HttpGet("Edit")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await new CompanyBusiness().GetCompanyByIdAsync(Id);
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Company" });

            if (model.Id != User?.GetOrganisationIdOrThrow())
                return Forbid();

            await PopulateOrganisationAddresses(model, model.Id);
            return View(model);
        }

        [HttpPost("Edit")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateOrganisationAddresses(model, model.Id);
                return View(model);
            }
            await new CompanyBusiness().UpdateCompanyAsync(model, User?.Identity?.Name!);
            return RedirectToAction("Info", "Companies", new { Id = UrlEncryptionBusiness.EncryptParam(model.Id.ToString()) });
        }

        [HttpGet("Import")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        public IActionResult Import(string? callBack)
        {
            var model = new CompaniesImportViewModel { callBack = callBack };
            return View(model);
        }

        [HttpPost("LinkAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkAddress(LinkAddressRequest request)
        {
            var result = await new CompanyBusiness()
                .LinkCompanyAddressesAsync(request, User.Identity!.Name!);

            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost("UnlinkAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkAddress(string companyId, Guid entityAddressId)
        {
            var result = await new CompanyBusiness()
                .UnlinkCompanyAddressAsync(entityAddressId, User.Identity!.Name!);

            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Couldn't unlink this address. Please try again.";

            return RedirectToAction("Info", new { Id = companyId });
        }


        [HttpPost("RemoveAddressLink")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAddressLink(string companyId, Guid addressId)
        {
            var result = await new CompanyBusiness()
                .RemoveCompanyAddressLink(companyId, addressId);

            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Couldn't unlink this address. Please try again.";

            return RedirectToAction("Info", new { Id = companyId });
        }

        [HttpPost("DeleteCompany")]
        [ValidateAntiForgeryToken]
        [MVCDecryptFilter]
        public async Task<IActionResult> DeleteCompany(string companyId)
        {
            var result = await new CompanyBusiness()
                .DeleteCompany(companyId, User.Identity?.Name!);

            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Couldn't delete this company. Please try again.";

            return RedirectToAction("Info", new { Id = companyId });
        }

        #region
        private async Task PopulateOrganisationAddresses<T>(T? model, Guid? companyId = null) where T : class, IHasOrganisationAddresses
        {
            if (model is null) return;

            model.OrganisationAddresses = await new AddressBusiness()
                .GetAvailableAddressesForCompanyAsync(User?.Identity?.Name!, companyId);
        }
        #endregion
    }
}
