
namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class AntiforgeryModule
{
    public static IServiceCollection AddAntiforgeryModule(this IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.Name = "fsm.af";
            options.HeaderName = "X-XSRF-TOKEN";
        });

        return services;
    }
}