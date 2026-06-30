using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Data;

namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class DatabaseModule
{
    public static IServiceCollection AddDatabaseModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //ServicesExtensions.FSMConnectionString =
        //    configuration.GetConnectionString("FSMConnectionString")
        //    ?? throw new InvalidOperationException("Connection string 'FSMConnectionString' not found.");
        services.FSMConnectionStringService(AppSettings.GetFSMConnectionString());
        services.AddDbContext<DataContext>();

        return services;
    }
}