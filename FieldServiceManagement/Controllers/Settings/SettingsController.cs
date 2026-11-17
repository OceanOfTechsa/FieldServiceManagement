using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers.Settings
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
