using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class FeaturesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
