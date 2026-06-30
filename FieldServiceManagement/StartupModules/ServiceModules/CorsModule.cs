using FieldServiceManagement.Business.Configuration;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public static class CorsModule
    {
        public static IServiceCollection AddCorsModule(this IServiceCollection services)
        {
            var baseUrl = AppSettings.baseUrl; // ← resolve once

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins(baseUrl)
                        .WithMethods("GET", "HEAD", "PUT", "PATCH", "POST", "DELETE")
                        .WithHeaders("Content-Type", "Accept"));

                options.AddPolicy("ReportGetPolicy", builder =>
                    builder.WithOrigins(baseUrl)
                        .WithMethods("GET")
                        .WithHeaders("Content-Type", "Accept"));

                options.AddPolicy("AllowAllGet", builder =>
                    builder.AllowAnyOrigin()
                        .WithMethods("GET")
                        .WithHeaders("Content-Type", "Accept"));
            });

            return services;
        }
    }
}
