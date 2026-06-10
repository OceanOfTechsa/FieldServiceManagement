using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FieldServiceManagement.Enum
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this System.Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = field?.GetCustomAttribute<DisplayAttribute>();

            return attribute?.Name ?? value.ToString();
        }


        public static string ResolveUserRoleName(int userRoleId)
        {
            return userRoleId switch
            {
                (int)UserRole.SuperAdmin => "Super Admin",
                (int)UserRole.Administrator => "Administrator",
                (int)UserRole.LimitedFieldAgent => "Limited Field Agent",
                (int)UserRole.FieldAgent => "Field Agent",
                (int)UserRole.Dispatcher => "Dispatcher",
                (int)UserRole.CallCenterAgent => "Call Center Agent",
                _ => "Unknown"
            };
        }
    }
}
