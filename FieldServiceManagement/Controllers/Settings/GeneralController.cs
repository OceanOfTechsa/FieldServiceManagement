using FieldServiceManagement.Business;
using FieldServiceManagement.Helpers;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers.Settings
{
    [Authorize(Roles = "SuperAdmin, Administrator")]
    public class GeneralController : Controller
    {
        public async Task<IActionResult> Organisation()
        {
            var model = await new OrganisationBusiness().GetOrganisationDeatailsByIdAsync(User.GetOrganisationIdOrThrow());
            if (model is null)
                return View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Organisation", ReturnUrl = "/Identity/Account/Manage/Settings/Organisation" });

            return View(model);
        }
    }
}
