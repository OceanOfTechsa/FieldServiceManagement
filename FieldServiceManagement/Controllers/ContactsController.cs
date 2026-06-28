using FieldServiceManagement.Business.AddressBusiness;
using FieldServiceManagement.Business.CompanyBusiness;
using FieldServiceManagement.Business.ContactBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Contact;
using FieldServiceManagement.ViewModels.Interfaces;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace FieldServiceManagement.Controllers
{
    [Route("Customers/[controller]")]
    public class ContactsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            var contacts = await new ContactBusiness().GetContactsByUserEmailAsync(User?.Identity?.Name!);
            if (!contacts.Any())
                return View("~/Views/Shared/PageEmptyState.cshtml", PageEmptyStates.Contacts);
            
            ViewBag.SelectedStatus = status;
            return View(contacts);
        }

        [HttpGet("Create")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateContactViewModel();
            await PopulateOrganisationAddresses(model);
            model.Companies = await new CompanyBusiness().GetCompaniesByUserEmailAsync(User?.Identity?.Name!);
            return View(model);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContactViewModel model)
        {

            if (!ModelState.IsValid)
            {
                await PopulateOrganisationAddresses(model);
                model.Companies = await new CompanyBusiness().GetCompaniesByUserEmailAsync(User?.Identity?.Name!);
                return View(model);
            }
            var contactId = await new ContactBusiness().CreateContactAsync(model, User?.Identity?.Name!);
            return RedirectToAction("Info", new { Id = UrlEncryptionBusiness.EncryptParam(contactId.ToString()!) });
        }

        [HttpGet("Import")]
        public IActionResult Import()
        {
            return View();
        }

        [HttpGet("Info")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Info(int? page, Guid Id)
        {
            var model = await new ContactBusiness().GetContactFullDetailsByIdAsync(Id);
            if (!model.Any())
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Contact" });

            return View(model.ToPagedList(page ?? 1, 10));
        }

        private async Task PopulateOrganisationAddresses<T>(T? model) where T : class, IHasOrganisationAddresses
        {
            if (model is null) return;
            model.OrganisationAddresses = await new AddressBusiness().GetAllAddressesByOrganisationIdAsync(User?.Identity?.Name!);
        }
    }
}
