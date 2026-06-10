namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class SwaggerModule
{
    public static IServiceCollection AddSwaggerModule(
        this IServiceCollection services,
        IWebHostEnvironment env)
    {
        if (!env.IsDevelopment()) return services;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "Field Service Management API",
                Version = "v1"
            });
        });

        return services;
    }
}