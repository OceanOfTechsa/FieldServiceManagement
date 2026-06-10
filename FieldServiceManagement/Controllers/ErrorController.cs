using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorController(ILogger<ErrorController> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    [Route("Error/{statusCode}")]
    public IActionResult HandleStatusCode(int statusCode, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["RequestId"] = HttpContext.TraceIdentifier;

        return statusCode switch
        {
            403 => View("403"),
            404 => View("404"),
            500 => View("500"),
            _ => View("404")
        };
    }

    [Route("Error/500")]
    public IActionResult HandleException()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature?.Error != null)
        {
            _logger.LogError(exceptionFeature.Error,
                "Unhandled exception. Path: {Path}, TraceId: {TraceId}",
                exceptionFeature.Path,
                HttpContext.TraceIdentifier);

            if (_env.IsDevelopment())
            {
                ViewData["ErrorMessage"] = exceptionFeature.Error.Message;
                ViewData["Path"] = exceptionFeature.Path;

                ViewData["StackTrace"] =
                    exceptionFeature.Error.StackTrace?.Length > 2000
                        ? exceptionFeature.Error.StackTrace[..2000]
                        : exceptionFeature.Error.StackTrace;
            }
        }

        ViewData["RequestId"] = HttpContext.TraceIdentifier;

        return View("500");
    }
}