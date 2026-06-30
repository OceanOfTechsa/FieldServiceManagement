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
            var connStr = AppSettings.GetFSMConnectionString();
            var elmahConnStr = env.IsDevelopment() ? connStr : connStr[..^34];

            services.AddElmah<SqlErrorLog>(options =>
            {
                options.OnPermissionCheck = context => context.User.IsInRole("SuperAdmin");
                options.Path = "elmah";
                options.ConnectionString = elmahConnStr;
                options.ApplicationName = "FieldServiceManagement";
            });

            services.AddSingleton<IErrorFilter, IgnoreNoiseFilter>();
            return services;
        }
    }
}
