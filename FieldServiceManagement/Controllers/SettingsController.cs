using FieldServiceManagement.Business.SettingsBusiness;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Settings;
using FieldServiceManagement.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("Admin/[controller]")]
    public class SettingsController : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var business = new SettingsBusiness();
            var model = await business.GetSearchByList(string.Empty, 1);
            return model == null ? View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Settings", Title = "Settings Not Found" }) : View(model);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(SettingsViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            new SettingsBusiness().AddNewSettings(model);
            return RedirectToAction("Index");
        }

        [HttpGet("Edit")]
        [MVCDecryptFilter]
        public async Task<IActionResult> Edit(int Id)
        {
            var business = new SettingsBusiness();
            var model = business.GetSettingsById(Id);
            return model == null ? View("ResourceNotFound", new ResourceNotFoundViewModel { ResourceName = "Setting", Title = "Setting Not Found" }) : View(model);
        }

        [HttpPost("Edit")]
        public ActionResult Edit(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var business = new SettingsBusiness();
                business.EditSettings(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
