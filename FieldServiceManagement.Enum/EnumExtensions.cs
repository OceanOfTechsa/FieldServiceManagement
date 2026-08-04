using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

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

        public static string GetNormalisedDisplayName(this System.Enum value)
        {
            var displayName = GetDisplayName(value);
            return Regex.Replace(displayName, "(?<=[a-z])(?=[A-Z])", " ");
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
                (int)UserRole.CustomerPortalUser => "Customer Portal User",
                _ => "Unknown"
            };
        }

        public static string ResolveStatusCssClass(this UserStatus status)
        {
            return status switch
            {
                UserStatus.Active => "bg-success",
                UserStatus.Invited => "bg-warning",
                UserStatus.InActive => "bg-secondary",
                UserStatus.Deleted => "bg-danger",
                _ => "bg-secondary"
            };
        }

        public static string GetNormalisedDisplayName(this SubscriptionPlanEnum plan) => plan switch
        {
            SubscriptionPlanEnum.Free => "Free",
            SubscriptionPlanEnum.Starter => "Starter",
            SubscriptionPlanEnum.Professional => "Professional",
            SubscriptionPlanEnum.Enterprise => "Enterprise",
            _ => plan.ToString()
        };

        public static string ResolvePlanCssClass(this SubscriptionPlanEnum plan) => plan switch
        {
            SubscriptionPlanEnum.Free => "fsm-tag--free",
            SubscriptionPlanEnum.Starter => "fsm-tag--starter",
            SubscriptionPlanEnum.Professional => "fsm-tag--professional",
            SubscriptionPlanEnum.Enterprise => "fsm-tag--enterprise",
            _ => "fsm-tag--starter"
        };
    }
}