using FieldServiceManagement.Business.Configuration;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace FieldServiceManagement.StartupModules.Middleware
{
    public sealed class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                // ----------------------------
                // Basic security headers
                // ----------------------------
                context.Response.Headers["referrer-policy"] = new StringValues("strict-origin-when-cross-origin");
                context.Response.Headers["x-content-type-options"] = new StringValues("nosniff");
                context.Response.Headers["x-frame-options"] = new StringValues("sameorigin");
                context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = new StringValues("none");
                context.Response.Headers["x-xss-protection"] = new StringValues("1; mode=block");

                // ----------------------------
                // Environment detection (KEEP AppSettings)
                // ----------------------------
                var env = AppSettings.EnvironmentName ?? "";
                var isDev = env.IndexOf("Development", StringComparison.OrdinalIgnoreCase) >= 0;

                var csp = new StringBuilder();

                // ----------------------------
                // Base CSP rules
                // ----------------------------
                csp.Append("default-src 'self';");
                csp.Append("base-uri 'self';");
                csp.Append("object-src 'none';");
                csp.Append("frame-ancestors 'none';");
                csp.Append("block-all-mixed-content;");

                // ----------------------------
                // Styles
                // ----------------------------
                csp.Append(
                    "style-src 'self' 'unsafe-inline' " +
                    "https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com https://fonts.googleapis.com;"
                );

                // ----------------------------
                // Fonts
                // ----------------------------
                csp.Append(
                    "font-src 'self' data: " +
                    "https://fonts.gstatic.com https://cdnjs.cloudflare.com https://unpkg.com;"
                );

                // ----------------------------
                // Images
                // ----------------------------
                csp.Append(
                    "img-src 'self' data: " +
                    "https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com https://code.jquery.com;"
                );

                // ----------------------------
                // Scripts
                // ----------------------------
                csp.Append(
                    "script-src 'self' 'unsafe-inline' 'unsafe-eval' " +
                    "https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com;"
                );

                csp.Append(
                    "script-src-elem 'self' 'unsafe-inline' " +
                    "https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com;"
                );

                csp.Append("script-src-attr 'self' 'unsafe-inline';");

                // ----------------------------
                // Forms
                // ----------------------------
                csp.Append("form-action 'self';");

                // ----------------------------
                // API / backend connections
                // ----------------------------
                csp.Append("connect-src 'self' ");

                // FSM backend APIs
                if (!string.IsNullOrWhiteSpace(AppSettings.BaseUrl))
                    csp.Append(AppSettings.BaseUrl + " ");

                if (!string.IsNullOrWhiteSpace(AppSettings.AzureBlobUri))
                    csp.Append(AppSettings.AzureBlobUri + " ");

                // ----------------------------
                // Dev support
                // ----------------------------
                if (isDev)
                {
                    csp.Append("http://localhost:* https://localhost:* ws://localhost:* wss://localhost:* ");
                    csp.Append("http://127.0.0.1:* https://127.0.0.1:* ws://127.0.0.1:* wss://127.0.0.1:* ");
                }

                // ----------------------------
                // CSP Reporting
                // ----------------------------
                csp.Append("report-uri /csp-report;");

                // ----------------------------
                // Apply CSP
                // ----------------------------
                if (isDev)
                {
                    context.Response.Headers["Content-Security-Policy-Report-Only"] = new StringValues(csp.ToString());
                    context.Response.Headers.Remove("Content-Security-Policy");
                }
                else
                {
                    context.Response.Headers["Content-Security-Policy"] = new StringValues(csp.ToString());
                    context.Response.Headers.Remove("Content-Security-Policy-Report-Only");

                    // ----------------------------
                    // Extra production security
                    // ----------------------------
                    context.Response.Headers["Strict-Transport-Security"] =
                        new StringValues("max-age=31536000; includeSubDomains");
                }

                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}