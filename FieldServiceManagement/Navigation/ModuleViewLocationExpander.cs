
using Microsoft.AspNetCore.Mvc.Razor;

namespace FieldServiceManagement.StartupModules.ServiceModules;

public class ModuleViewLocationExpander : IViewLocationExpander
{
    private const string ModulePathKey = "modulePath";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        if (context.ActionContext.ActionDescriptor.RouteValues
            .TryGetValue(ModulePathKey, out var modulePath)
            && !string.IsNullOrWhiteSpace(modulePath))
        {
            context.Values[ModulePathKey] = modulePath;
        }
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        if (context.Values.TryGetValue(ModulePathKey, out var modulePath)
            && !string.IsNullOrWhiteSpace(modulePath)
            && context.ActionContext.ActionDescriptor.RouteValues
                .TryGetValue("controller", out var controller))
        {
            yield return $"/Views/{modulePath}/{controller}/{{0}}.cshtml";
            yield return $"/Views/{modulePath}/{{0}}.cshtml";
        }

        foreach (var location in viewLocations)
            yield return location;
    }
}
