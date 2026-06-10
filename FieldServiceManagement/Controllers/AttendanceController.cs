using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
