using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("/Admin/Workflow")]
    public class WorkflowController : Controller
    {
        [HttpGet("Stages")]
        public IActionResult Stages()
        {
            return View();
        }

        [HttpGet("Approvers")]
        public IActionResult Approvers()
        {
            return View();
        }
    }
}
