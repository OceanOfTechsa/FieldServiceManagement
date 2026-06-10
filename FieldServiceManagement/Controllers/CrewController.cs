using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Route("WorkOrderManagement/[controller]")]
    public class CrewController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("Edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
