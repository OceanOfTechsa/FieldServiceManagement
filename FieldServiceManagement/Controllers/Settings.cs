using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class Settings : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }
    }
}
