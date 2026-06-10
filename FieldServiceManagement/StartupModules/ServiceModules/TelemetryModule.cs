using FieldServiceManagement.Business.Configuration;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public static class TelemetryModule
    {
        public static IServiceCollection AddTelemetryModule(this IServiceCollection services, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                var options = new ApplicationInsightsServiceOptions
                {
                    ConnectionString = "InstrumentationKey=" + AppSettings.InstrumentationKey
                };

                services.AddApplicationInsightsTelemetry(options);
                services.AddSnapshotCollector();
            }

            return services;
        }
    }
}
