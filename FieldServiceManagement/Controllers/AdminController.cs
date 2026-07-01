using FieldServiceManagement.Business.HealthCheckBusiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        // GET: /Admin/Health
        [HttpGet("Health")]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Admin/Health/Info/Database
        [HttpGet("Health/Info/{service}")]
        public IActionResult Info(string service)
        {
            return View("Info", service);
        }

        [HttpGet("PendingAlerts")]
        public IActionResult PendingAlerts()
        {
            return View();
        }

        [HttpGet("ViewAllAlerts")]
        public IActionResult ViewAllAlerts()
        {
            return View();
        }

        [HttpGet("ResendEmail")]
        public IActionResult ResendEmail()
        {
            return View();
        }

        [HttpGet("SQLFirewall")]
        public IActionResult SQLFirewall()
        {
            return View();
        }

        [HttpGet("FlushCache")]
        public IActionResult FlushCache()
        {
            return View();
        }
    }
}