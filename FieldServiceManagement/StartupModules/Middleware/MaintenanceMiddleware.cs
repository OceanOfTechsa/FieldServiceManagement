using FieldServiceManagement.StartupModules.Middleware;

namespace FieldServiceManagement.StartupModules.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _isMaintenanceMode;

        public MaintenanceMiddleware(RequestDelegate next, bool isMaintenanceMode)
        {
            _next = next;
            _isMaintenanceMode = isMaintenanceMode;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow access to Maintenance page itself + static files + areas you want to keep open
            if (_isMaintenanceMode &&
                !context.Request.Path.StartsWithSegments("/Maintenance") &&
                !context.Request.Path.StartsWithSegments("/lib") &&      // static files
                !context.Request.Path.StartsWithSegments("/css") &&
                !context.Request.Path.StartsWithSegments("/js") &&
                !context.Request.Path.StartsWithSegments("/images"))
            {
                context.Response.Redirect("/Maintenance");
                return;
            }

            await _next(context);
        }
    }
}


public static class MaintenanceMiddlewareExtensions
{
    public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder builder, bool isMaintenanceMode)
    {
        return builder.UseMiddleware<MaintenanceMiddleware>(isMaintenanceMode);
    }
}