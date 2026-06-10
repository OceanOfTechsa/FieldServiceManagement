namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class SecurityModule
{
    public static IServiceCollection AddSecurityModule(this IServiceCollection services)
    {
        services.Configure<IISServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });

        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(90);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            options.HttpsPort = 443;
        });

        return services;
    }
}