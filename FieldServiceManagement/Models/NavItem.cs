using FieldServiceManagement.Enum;

namespace FieldServiceManagement.Models
{
    public class NavItem
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = "#";
        public string Icon { get; set; } = string.Empty;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public List<NavSubItem> Subs { get; set; } = new();
    }

    public class NavSubItem
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = "#";
        public string[] Roles { get; set; } = Array.Empty<string>();


        // ── plan gating + badges ───────────────────────────────────
        public SubscriptionPlanEnum RequiredPlan { get; set; } = SubscriptionPlanEnum.Free;
        public bool IsNew { get; set; }
        public string NewText { get; set; } = string.Empty;
        public bool HasUpdate { get; set; }
        public string UpdateText { get; set; } = string.Empty;
    }
}