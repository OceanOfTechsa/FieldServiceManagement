using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class AddressesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(string? returnUrl)
        {
           if(!string.IsNullOrEmpty(returnUrl))
           {
             return Redirect(returnUrl);
           }
           return View();
        }
    }
}
