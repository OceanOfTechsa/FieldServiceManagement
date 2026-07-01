using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    public class UserManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
