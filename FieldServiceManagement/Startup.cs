using FieldServiceManagement.StartupModules.ServiceModules;
using FieldServiceManagement.Web.StartupModules.PipelineModules;

namespace FieldServiceManagement;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        WebHostEnvironment = env;

        // If you rely on env-specific appsettings, the host already loads these by default.
        // Keep this only if you have custom loading logic elsewhere.
        _ = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment WebHostEnvironment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddDatabaseModule(Configuration)
            .AddIdentityModule()
            .AddAuthenticationModule()
            .AddAntiforgeryModule()
            .AddMvcModule(WebHostEnvironment)
            .AddCorsModule()
            .AddResponseCaching()
            .AddTelemetryModule(WebHostEnvironment)
            //.AddReportingModule(WebHostEnvironment)
            .AddSecurityModule()
            .AddSwaggerModule(WebHostEnvironment)
            .AddCacheModule(WebHostEnvironment)
            .AddElmahModule(WebHostEnvironment)
            .AddResendModule(WebHostEnvironment)
            .AddHealthModule();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ResendModule.BindAccessor(app.ApplicationServices);
        app.UseFSMPipeline(env);
    }
}