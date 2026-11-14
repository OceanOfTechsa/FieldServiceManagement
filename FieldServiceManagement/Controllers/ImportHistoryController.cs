using FieldServiceManagement.Business;
using FieldServiceManagement.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Route("ImportHistory")]
    [Authorize(Roles = "Administrator")]
    public class ImportHistoryController : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> Index(string? status, string? module)
        {
            ViewBag.Module = module ?? "Unknown";
            ViewBag.SelectedStatus = status;
            var history = await new DataImportBusiness().GetHistoryAsync(User.GetOrganisationIdOrThrow(), module);
            return View(history);
        }

        [HttpGet("{id:guid}/Errors")]
        public async Task<IActionResult> Errors(Guid id)
        {
            var path = await new DataImportBusiness().GetErrorFileAsync(id, User.Identity?.Name!);

            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
                return NotFound();

            return PhysicalFile(path, "text/csv", "import-errors.csv");
        }
    }
}
