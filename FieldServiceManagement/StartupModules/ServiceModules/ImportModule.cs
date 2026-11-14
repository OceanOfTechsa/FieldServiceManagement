using FieldServiceManagement.Business.Imports;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public static class ImportModule
    {
        public static IServiceCollection AddImportModule(this IServiceCollection services)
        {
            services.AddHostedService<ContactImportWorker>();
            return services;
        }
    }
}