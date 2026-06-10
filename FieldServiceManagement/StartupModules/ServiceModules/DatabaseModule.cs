using FieldServiceManagement.Data;

namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class DatabaseModule
{
    public static IServiceCollection AddDatabaseModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ServicesExtensions.FieldServiceManagementConnectionString =
            configuration.GetConnectionString("FieldServiceManagement");

        services.AddDbContext<DataContext>();

        return services;
    }
}