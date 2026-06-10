using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
