using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
