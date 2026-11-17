using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers.WorkForce
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
