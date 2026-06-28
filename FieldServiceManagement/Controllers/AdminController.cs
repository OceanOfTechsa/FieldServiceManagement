using FieldServiceManagement.Business.HealthCheckBusiness;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("Admin/Health")]
    public class AdminController : Controller
    {
        [HttpGet("")]
        public IActionResult Dashboard() => View();

        [HttpGet("Info/{service}")]
        public IActionResult Detail(string service) => View("Detail", service);
    }
}