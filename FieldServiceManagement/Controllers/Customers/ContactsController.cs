using FieldServiceManagement.Business.AddressBusiness;
using FieldServiceManagement.Business.CompanyBusiness;
using FieldServiceManagement.Business.ContactBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Contact;
using FieldServiceManagement.ViewModels.Interfaces;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers.Customers
{
    [Authorize]
    public class ContactsController : Controller
    {
        public async Task<IActionResult> Index(string? status)
        {
            var contacts = await new ContactBusiness().GetContactsByUserEmailAsync(User.Identity!.Name!);
            if (!contacts.Any())
                return View("~/Views/Shared/PageEmptyState.cshtml", PageEmptyStates.Contacts);
            
            ViewBag.SelectedStatus = status;
            return View(contacts);
        }

        [MVCDecryptFilter]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        public async Task<IActionResult> Create(Guid? companyId)
        {
            var model = new CreateContactViewModel { CompanyId = companyId ?? Guid.Empty };
            await PopulateOrganisationAddresses(model, User.GetOrganisationIdOrThrow());
            await PopulateCompanies(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        public async Task<IActionResult> Create(CreateContactViewModel model)
        {

            if (!ModelState.IsValid)
            {
                await PopulateOrganisationAddresses(model, User.GetOrganisationIdOrThrow());
                await PopulateCompanies(model);
                return View(model);
            }
            var contactId = await new ContactBusiness().CreateContactAsync(model, User.Identity?.Name!);
            return RedirectToAction("Info", new { Id = UrlEncryptionBusiness.EncryptParam(contactId.ToString()!) });
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Import()
        {
            var currentUser = await new UserBusiness().GetUserDetailsByUserNameAsync(User.Identity?.Name!);
            var limits = await new ContactBusiness().GetContactImportLimitsAsync(currentUser);
            ViewBag.ImportMaxFileBytes = limits.MaxFileBytes;
            ViewBag.ImportMaxRecords = limits.MaxRecords;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(16 * 1024 * 1024)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Import(ContactImportRequest request)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m))
                    ?? "The import request is invalid.";

                return BadRequest(new { success = false, message });
            }

            var result = await new ContactBusiness().StartContactImportAsync(request, User.Identity?.Name!);

            if (!result.Success) return BadRequest(new { success = false, message = result.Message });

            return Accepted(new{success = true,importId = result.ImportId,message = result.Message});
        }

        [MVCDecryptFilter]
        public async Task<IActionResult> Info(int? page, Guid Id)
        {
            var model = await new ContactBusiness().GetContactFullDetailsByIdAsync(Id, User.Identity?.Name!);
            if (model?.Contact is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Contact" });

            var orgId = User!.GetOrganisationIdOrThrow();

            if (model.Contact.OrganisationId != orgId)
                return Forbid();

            model.OrganisationAddresses = await new AddressBusiness().GetAvailableAddressesForCompanyAsync(orgId, model.Company.Id);

            return View(model);
        }

        [MVCDecryptFilter]
        [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = await new ContactBusiness().GetContactByIdAsync(Id);
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Contact" });

            var OrgId = User.GetOrganisationIdOrThrow();

            if (model.OrganisationId != OrgId)
                return Forbid();

            await PopulateOrganisationAddresses(model, OrgId);
            await PopulateCompanies(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkAddress(LinkAddressRequest request)
        {
            var result = await new AddressBusiness().LinkEntityAddressesAsync("Contact",request, User.Identity!.Name!);

            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkAddress(string contactId, Guid entityAddressId)
        {
            Guid contactGuid;
            try
            {
                contactGuid = Guid.Parse(UrlEncryptionBusiness.DecryptParam(contactId));
            }
            catch
            {
                TempData["ErrorMessage"] = "Invalid reference.";
                return RedirectToAction("Index");
            }

            var result = await new AddressBusiness()
                .UnlinkEntityAddressAsync("Contact", contactGuid, entityAddressId, User?.Identity?.Name!);

            if (!result.Success)
                TempData["ErrorMessage"] = result.Message ?? "Couldn't unlink this address. Please try again.";

            return RedirectToAction("Info", new { Id = contactId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAddressLink(string contactId, Guid addressId)
        {
            var result = await new AddressBusiness()
                .RemoveEntityAddressLinkAsync(contactId, addressId, User!.GetOrganisationIdOrThrow());

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message ?? "Couldn't remove the address link. Please try again." });

            return Ok(new { success = true });
        }
        
         [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeleteContact()
         {
             var contactId = Request.Form["contactId"].ToString();
             var result = await new ContactBusiness()
                 .DeleteContact(contactId, User.Identity?.Name!);
          
             if (!result.Success)
                 return BadRequest(new { success = false, message = result.Message ?? "Couldn't delete this contact. Please try again." });
          
             return Ok(new { success = true });
         }
         
         [HttpPost]
         [ValidateAntiForgeryToken]
         [Authorize(Roles = "SuperAdmin, Administrator, CallCenterAgent")]
         public async Task<IActionResult> Edit(ContactViewModel model)
         {
             if (!ModelState.IsValid)
             {
                 await PopulateOrganisationAddresses(model, model.Id);
                 await PopulateCompanies(model);
                 ModelState.AddModelError(string.Empty, "Couldn't save the contact. Please try again.");
                 return View(model);
             }
             
             var result = await new ContactBusiness().UpdateContactAsync(model, User?.Identity?.Name!);
             return RedirectToAction("Info", "Contacts",new { Id = UrlEncryptionBusiness.EncryptParam(model.Id.ToString()) });
         }

        #region PRIVATE METHODS

        private async Task PopulateOrganisationAddresses<T>(T? model, Guid organisationId) where T : class, IHasOrganisationAddresses
        {
            if (model is null) return;
            model.OrganisationAddresses = await new AddressBusiness().GetAllAddressesByOrganisationIdAsync(organisationId);
        }

        private async Task PopulateCompanies<T>(T? model) where T : class, IHasCompanies
        {
            if (model is null) return;
            var companies = await new CompanyBusiness().GetCompaniesByUserEmailAsync(User?.Identity?.Name!);
            model.Companies = companies.Where(c => c.IsDeleted == false);
        }
        #endregion
    }
}
