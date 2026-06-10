//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using System;
//using System.IO;
//using Telerik.Reporting.Cache.File;
//using Telerik.Reporting.Services;
//using Telerik.WebReportDesigner.Services;

//namespace FormsManagement.StartupModules.ServiceModules
//{
//    public static class ReportingModule
//    {
//        public static IServiceCollection AddReportingModule(this IServiceCollection services, IWebHostEnvironment env)
//        {
//            var reportsPath = Path.Combine(env.ContentRootPath, "wwwroot", "Reports");

//            services.TryAddSingleton<IReportServiceConfiguration>(sp =>
//                new ReportServiceConfiguration
//                {
//                    ReportingEngineConfiguration = sp.GetService<IConfiguration>(),
//                    HostAppId = "forms.dut.ac.za",
//                    Storage = new FileStorage(),
//                    ReportSourceResolver = new TypeReportSourceResolver().AddFallbackResolver(
//                        new UriReportSourceResolver(reportsPath))
//                });

//            services.TryAddSingleton<IReportDesignerServiceConfiguration>(sp =>
//                new ReportDesignerServiceConfiguration
//                {
//                    DefinitionStorage = new FileDefinitionStorage(reportsPath),
//                    SettingsStorage = new FileSettingsStorage(
//                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telerik Reporting")),
//                    ResourceStorage = new ResourceStorage(Path.Combine(reportsPath, "Resources"))
//                });

//            return services;
//        }
//    }
//}
