using FieldServiceManagement.Business;
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

namespace FieldServiceManagement.Controllers.Customers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        public async Task<IActionResult> Index(string? status)
        {
            var companies = await new CompanyBusiness().GetCompaniesByUserEmailAsync(User?.Identity?.Name!);
            if (!companies.Any())
                return View("~/Views/Shared/PageEmptyState.cshtml", PageEmptyStates.Companies);
            
            ViewBag.SelectedStatus = status;
            return View(companies);
        }

        [MVCDecryptFilter]
        public async Task<IActionResult> Info(int? page, Guid Id)
        {
            var Model = await new CompanyBusiness().GetCompanyFullDetailsByIdAsync(Id, User?.Identity?.Name!);
            if (Model.Company is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Company" });

            if (Model.Company.OrganisationId != User?.GetOrganisationIdOrThrow())
                return Forbid();

            Model.OrganisationAddresses = await new AddressBusiness().GetAvailableAddressesForCompanyAsync(User.GetOrganisationIdOrThrow(), Model.Company.Id);
            return View(Model);
        }

        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        public async Task<IActionResult> Create(string? returnUrl)
        {
            var model = new CreateCompanyViewModel{ReturnUrl = returnUrl};
            await PopulateOrganisationAddresses(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
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

        [MVCDecryptFilter]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
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

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Import(string? callBack)
        {
            var (MaxFileBytes, MaxRecords) = await new DataImportBusiness().GetImportLimitsAsync(User.GetOrganisationIdOrThrow());
            ViewBag.ImportMaxFileBytes = MaxFileBytes;
            ViewBag.ImportMaxRecords = MaxRecords;

            return View(new CompaniesImportViewModel { callBack = callBack });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(16 * 1024 * 1024)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Import(CompanyImportRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "The import request is invalid.";

                return BadRequest(new { success = false, message });
            }

            var result = await new CompanyBusiness().StartCompanyImportAsync(request, User.Identity?.Name!);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            return Accepted(new { success = true, importId = result.ImportId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkAddress(LinkAddressRequest request)
        {
            var result = await new AddressBusiness().LinkEntityAddressesAsync("Contact", request, User.Identity!.Name!);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkAddress(string companyId, Guid entityAddressId)
        {
            var compId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(companyId));

            var result = await new AddressBusiness()
                .UnlinkEntityAddressAsync("Company", compId, entityAddressId, User?.Identity.Name!);

            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Couldn't unlink this address. Please try again.";

            return RedirectToAction("Info", new { Id = companyId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAddressLink(string companyId, Guid addressId)
        {
            var result = await new AddressBusiness()
                .RemoveEntityAddressLinkAsync(companyId, addressId, User!.GetOrganisationIdOrThrow());

            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Couldn't unlink this address. Please try again.";

            return RedirectToAction("Info", new { Id = companyId });
        }

        [HttpPost]
        [MVCDecryptFilter]
        [ValidateAntiForgeryToken]
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
                .GetAvailableAddressesForCompanyAsync(User.GetOrganisationIdOrThrow(), companyId);
        }
        #endregion
    }
}
