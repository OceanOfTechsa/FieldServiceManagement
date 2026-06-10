using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class TripsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
