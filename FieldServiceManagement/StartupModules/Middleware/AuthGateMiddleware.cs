namespace FieldServiceManagement.Web.Middleware
{
    public class AuthGateMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthGateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // ✅ Allow Identity pages (login, logout, etc.)
            if (path.StartsWith("/identity"))
            {
                await _next(context);
                return;
            }

            // ✅ Allow static files (css, js, images)
            if (path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib") || path.StartsWith("/images") || path.StartsWith("/favicon"))
            {
                await _next(context);
                return;
            }

            // 🔒 If NOT authenticated → redirect to login
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.Redirect("/Identity/Account/Login");
                return;
            }

            // ✅ If authenticated → continue
            await _next(context);
        }
    }
}