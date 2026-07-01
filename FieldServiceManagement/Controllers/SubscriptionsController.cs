using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class SubscriptionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
