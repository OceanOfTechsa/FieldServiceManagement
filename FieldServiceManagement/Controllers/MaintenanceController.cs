using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class MaintenanceController : Controller
    {
        public IActionResult Index()
        {
            var model = new MaintenanceModel
            {
                isSystemMaintenance = AppSettings.isSystemMaintenance,
                MaintenanceEstimatedTime = AppSettings.systemMaintenanceEstimatedTime,
                MaintenanceMessage = AppSettings.systemMaintenanceNote
            };
            return View(model);
        }
    }
}
