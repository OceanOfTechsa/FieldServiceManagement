using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
