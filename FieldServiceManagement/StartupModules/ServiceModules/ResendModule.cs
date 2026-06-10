using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.Infrastructure;
using Resend;

public static class ResendModule
{
    public static IServiceCollection AddResendModule(
        this IServiceCollection services,
        IWebHostEnvironment env)
    {
        services.AddOptions();
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = env.IsDevelopment()
                ? Environment.GetEnvironmentVariable("RESEND_APITOKEN")!
                : AppSettings.GetResendApiToken();
        });
        services.AddTransient<IResend, ResendClient>();
        return services;
    }

    public static void BindAccessor(IServiceProvider services)
    {
        ServiceProviderAccessor.Initialise(services);
    }
}