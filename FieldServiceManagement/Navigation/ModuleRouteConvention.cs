
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace FieldServiceManagement.StartupModules.ServiceModules;

public class ModuleRouteConvention : IControllerModelConvention
{
    private const string ControllersNamespace =
        "FieldServiceManagement.Controllers.";

    public void Apply(ControllerModel controller)
    {
        var controllerNamespace = controller.ControllerType.Namespace;

        if (string.IsNullOrWhiteSpace(controllerNamespace) ||
            !controllerNamespace.StartsWith(
                ControllersNamespace,
                StringComparison.Ordinal))
        {
            return;
        }

        var namespacePath = controllerNamespace[
            ControllersNamespace.Length..]
            .Split('.');

        var controllerName = controller.ControllerName;

        // Avoid duplicating the controller name when it matches
        // the final namespace segment.
        var moduleSegments = namespacePath
            .TakeWhile((segment, index) =>
                index < namespacePath.Length - 1 ||
                !string.Equals(
                    segment,
                    controllerName,
                    StringComparison.OrdinalIgnoreCase));

        var modulePath = string.Join("/", moduleSegments);

        var routeSegments = namespacePath
            .Select(segment => segment)
            .ToList();

        if (!string.Equals(
                namespacePath[^1],
                controllerName,
                StringComparison.OrdinalIgnoreCase))
        {
            routeSegments.Add(controllerName);
        }

        var route = $"{string.Join("/", routeSegments)}/{{action=Index}}";

        controller.RouteValues["modulePath"] = modulePath;

        foreach (var selector in controller.Selectors)
        {
            if (selector.AttributeRouteModel is not null)
                continue;

            selector.AttributeRouteModel = new AttributeRouteModel
            {
                Template = route
            };
        }
    }
}