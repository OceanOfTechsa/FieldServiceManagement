using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class ReferenceDataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
