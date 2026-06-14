using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    [Route("Identity/Account/Manage/Settings")]
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
