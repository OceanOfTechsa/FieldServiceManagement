using ElmahCore;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using FieldServiceManagement.Business.Configuration;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public static class ElmahModule
    {
        public static IServiceCollection AddElmahModule(this IServiceCollection services, IWebHostEnvironment env)
        {
            // SQL-backed Elmah
            services.AddElmah<SqlErrorLog>(options =>
            {
                options.OnPermissionCheck = context => context.User.IsInRole("SuperAdmin");
                options.Path = "elmah";

                // Preserve your existing production trimming logic
                var connStr = AppSettings.GetFSMConnectionString();
                options.ConnectionString = env.IsDevelopment() ? connStr : connStr[..^34];

                options.ApplicationName = "FieldServiceManagement";
            });

            services.AddSingleton<IErrorFilter, IgnoreNoiseFilter>();

            return services;
        }
    }
}
