
using Microsoft.AspNetCore.Mvc.Razor;

namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class MvcModule
{
    public static IServiceCollection AddMvcModule(
        this IServiceCollection services,
        IWebHostEnvironment env)
    {
        var mvc = services.AddControllersWithViews(options =>
        {
            options.Conventions.Add(new ModuleRouteConvention());
        });

        mvc.AddRazorOptions(options =>
        {
            options.ViewLocationExpanders.Add(
                new ModuleViewLocationExpander()
            );
        });

        if (env.IsDevelopment())
        {
            mvc.AddRazorRuntimeCompilation(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(
                    new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                        env.ContentRootPath
                    )
                );
            })
            .AddNewtonsoftJson();
        }

        services.AddHttpContextAccessor();

        return services;
    }
}
