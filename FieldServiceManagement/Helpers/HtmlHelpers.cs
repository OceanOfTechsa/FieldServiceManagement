using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Ganss.Xss;

namespace FieldServiceManagement.Helpers
{
    public static class HtmlHelpers
    {
        public static IEnumerable<string> GetModelErrors(this ViewDataDictionary viewData)
        {
            return viewData.ModelState.Values
                .SelectMany(v => v.Errors)
                .Where(e => !string.IsNullOrEmpty(e.ErrorMessage))
                .Select(e => e.ErrorMessage);
        }

        public static string GetBrowser(this ViewDataDictionary viewData)
        {
            if (viewData.TryGetValue("Browser", out var browser))
            {
                return browser?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        public static string SanitizeInput(string input)
        {
            return new HtmlSanitizer().Sanitize(input);
        }
    }
}