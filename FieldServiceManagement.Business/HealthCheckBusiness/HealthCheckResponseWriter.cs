using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace FieldServiceManagement.Business.HealthCheckBusiness
{
    public static class HealthCheckResponseWriter
    {
        private static readonly JsonSerializerOptions _opts =
            new() { WriteIndented = true };

        // Used by /health/status — footer polling, no sensitive detail
        public static Task WriteStatusResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                durationMs = report.TotalDuration.TotalMilliseconds
            }, _opts));
        }

        // Used by /health/detail — SuperAdmin dashboard and detail pages
        public static Task WriteDetailedResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var result = new
            {
                status = report.Status.ToString(),
                durationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    durationMs = e.Value.Duration.TotalMilliseconds,
                    data = e.Value.Data,
                    error = e.Value.Exception?.Message
                })
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(result, _opts));
        }
    }
}