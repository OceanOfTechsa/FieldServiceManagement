using FieldServiceManagement.Business.HealthCheckBusiness;
using FieldServiceManagement.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public static class HealthModule
    {
        public static IServiceCollection AddHealthModule(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<ApplicationHealthCheck>(name: "Application",tags: ["app"])
                .AddCheck<DatabaseHealthCheck>("Database", tags: ["db"])
                .AddCheck<ResendHealthCheck>(name: "Resend",tags: ["email"])
                .AddCheck<RedisHealthCheck>(name: "Redis", tags: ["chache"]);
            return services;
        }

        public static IApplicationBuilder UseHealthModule(this IApplicationBuilder app)
        {
            // Runs AFTER UseEndpoints so MVC routes are never affected
            app.UseHealthChecks("/health/status", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteStatusResponse,
                AllowCachingResponses = false
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/health/detail"))
                {
                    if (!context.User.IsInRole("SuperAdmin"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }
                }
                await next();
            });

            app.UseHealthChecks("/health/detail", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter.WriteDetailedResponse,
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy]   = StatusCodes.Status200OK,
                    [HealthStatus.Degraded]  = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status200OK
                }
            });

            return app;
        }
    }
}