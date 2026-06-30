using FieldServiceManagement.Navigation;
using Microsoft.AspNetCore.Mvc.Razor;

namespace FieldServiceManagement.StartupModules.ServiceModules;

public static class MvcModule
{
    public static IServiceCollection AddMvcModule(
        this IServiceCollection services,
        IWebHostEnvironment env)
    {
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(
                new PrefixedViewLocationExpanderModule(ViewModules.All)
            );
        });

        var mvc = services.AddControllersWithViews();

        if (env.IsDevelopment())
            mvc.AddRazorRuntimeCompilation(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(
                    new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                        env.ContentRootPath));
            })
            .AddNewtonsoftJson();

        services.AddHttpContextAccessor();
        return services;
    }
}