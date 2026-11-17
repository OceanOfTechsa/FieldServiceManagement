using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers.Settings
{
    public class AccountController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }

        public async Task<IActionResult> Preferences()
        {
            return View();
        }

        public async Task<IActionResult> Security()
        {
            return View();
        }

        public async Task<IActionResult> Notifications()
        {
            return View();
        }

        public async Task<IActionResult> Integrations()
        {
            return View();
        }
    }
}
