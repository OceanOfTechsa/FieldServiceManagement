using FieldServiceManagement.StartupModules.ServiceModules;
using FieldServiceManagement.Web.StartupModules.PipelineModules;

namespace FieldServiceManagement;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        WebHostEnvironment = env;
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
            // .AddCacheModule(WebHostEnvironment)
            .AddElmahModule(WebHostEnvironment)
            .AddResendModule(WebHostEnvironment);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ResendModule.BindAccessor(app.ApplicationServices);
        app.UseFSMPipeline(env);
    }
}