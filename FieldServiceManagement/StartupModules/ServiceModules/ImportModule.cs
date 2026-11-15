using FieldServiceManagement.Business.Imports.Workers;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public static class ImportModule
    {
        public static IServiceCollection AddImportModule(this IServiceCollection services)
        {
            services.AddHostedService<ContactImportWorker>();
            services.AddHostedService<CompanyImportWorker>();
            return services;
        }
    }
}