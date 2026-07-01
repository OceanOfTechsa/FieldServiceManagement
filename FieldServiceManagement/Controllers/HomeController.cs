using FieldServiceManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FieldServiceManagement.Controllers;

[Authorize]
public class HomeController : Controller
{
    public ActionResult Index() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    
    public IActionResult About() => View();

    public IActionResult Offline(string? ReturnUrl)
    {
        return View((object)ReturnUrl!);
    }
}
