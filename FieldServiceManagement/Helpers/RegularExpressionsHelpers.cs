using System.Text.RegularExpressions;

namespace FieldServiceManagement.Helpers
{
    public static class RegularExpressionsHelpers
    {
        public static string SplitCamelCase(this string input) => Regex.Replace(input, "(?<=[a-z])(?=[A-Z])", " ");
    }
}
