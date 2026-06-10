using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class TimeOffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
