using FieldServiceManagement.Business.AddressBusiness;
using FieldServiceManagement.Business.CountryBusiness;
using FieldServiceManagement.Business.StateBusiness;
using FieldServiceManagement.ViewModels.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize]
    public class AddressesController : Controller
    {
     
        public async Task<IActionResult> Index(string? status)
        {
            var Addresses = await new AddressBusiness().GetAddressByUserEmailAsync(User?.Identity?.Name!);
            ViewBag.SelectedStatus = status;
            return View(Addresses);
        }

        public async Task<IActionResult> Create(string? returnUrl)
        {
            var model = await BuildCreateAddressModel(new CreateAddressViewModel(), returnUrl);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await BuildCreateAddressModel(model, model.ReturnUrl);
                return View(model);
            }

            var result = await new AddressBusiness().CreateAddress(model, User?.Identity?.Name!);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message!);
                model = await BuildCreateAddressModel(model, model.ReturnUrl);
                return View(model);
            }

            return Redirect(model.ReturnUrl ?? "/Addresses");
        }

        public IActionResult Info()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }



        #region PRIVATE METHODS
        private async Task<CreateAddressViewModel> BuildCreateAddressModel(CreateAddressViewModel model, string? returnUrl)
        {
            model.ReturnUrl = returnUrl;
            model.States = await new StateBusiness().GetAllStates();
            model.Countries = await new CountryBusiness().GetAllCountries();
            return model;
        }
        #endregion
    }
}
