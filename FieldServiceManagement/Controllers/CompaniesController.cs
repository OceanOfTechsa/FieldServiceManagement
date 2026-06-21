using FieldServiceManagement.Business.AddressBusiness;
using FieldServiceManagement.Business.CompanyBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Company;
using FieldServiceManagement.ViewModels.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

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
            var model = await new CompanyBusiness().GetCompanyFullDetailsByIdAsync(Id);
            if (!model.Any())
                return NotFound();

            return View(model.ToPagedList(page ?? 1, 10));
        }

        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        [HttpGet("Create")]
        public async Task<IActionResult> Create(string? returnUrl)
        {
            var model = new CreateCompanyViewModel();
            model.ReturnUrl = returnUrl;
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
            await PopulateOrganisationAddresses(model);
            return View(model);
        }

        [HttpPost("Edit")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateOrganisationAddresses(model);
                return View(model);
            }
            await new CompanyBusiness().UpdateCompanyAsync(model, User?.Identity?.Name!);
            return RedirectToAction("Info", "Companies", new { Id = UrlEncryptionBusiness.EncryptParam(model.Id.ToString()) });
        }

        [HttpGet("Import")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        public IActionResult Import(string? callBack)
        {
            var model = new CompaniesImportViewModel
            {
                callBack = callBack
            };
            return View(model);
        }

        private async Task PopulateOrganisationAddresses<T>(T? model) where T : class, IHasOrganisationAddresses
        {
            if (model is null) return;
            model.OrganisationAddresses = await new AddressBusiness().GetAllAddressesByOrganisationIdAsync(User?.Identity?.Name!);
        }
    }
}
