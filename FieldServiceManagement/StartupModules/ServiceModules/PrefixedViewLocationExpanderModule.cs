using Microsoft.AspNetCore.Mvc.Razor;

namespace FieldServiceManagement.StartupModules.ServiceModules
{
    public class PrefixedViewLocationExpanderModule : IViewLocationExpander
    {
        private readonly string[] _prefixes;

        public PrefixedViewLocationExpanderModule(params string[] prefixes)
        {
            _prefixes = prefixes;
        }

        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {
            var prefixedLocations = _prefixes
                .Select(prefix => $"/Views/{prefix}/{{1}}/{{0}}.cshtml");

            return prefixedLocations.Concat(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context) { }
    }
}
