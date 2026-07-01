using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class FormsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
