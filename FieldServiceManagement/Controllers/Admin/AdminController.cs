using FieldServiceManagement.Business.HealthCheckBusiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Info(string service)
        {
            return View(service);
        }

        public IActionResult PendingAlerts()
        {
            return View();
        }

        public IActionResult ViewAllAlerts()
        {
            return View();
        }

        public IActionResult ResendEmail()
        {
            return View();
        }

        public IActionResult SQLFirewall()
        {
            return View();
        }

        public IActionResult FlushCache()
        {
            return View();
        }
    }
}