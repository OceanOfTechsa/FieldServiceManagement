using FieldServiceManagement.Enum;

namespace FieldServiceManagement.Data.Extensions
{
    public static class Extensions
    {
        public static DateTime SaDateTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time"));
        }

        public static string GetFriendlyNameByFormEnum(this FormsEnum formName)
        {
            return System.Text.RegularExpressions.Regex.Replace(formName.ToString(), "([a-z])([A-Z])", "$1 $2");
        }
    }
}
