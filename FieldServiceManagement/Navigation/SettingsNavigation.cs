using FieldServiceManagement.Enum;
using FieldServiceManagement.Models;

namespace FieldServiceManagement.Web.Navigation
{
    public static class SettingsNavigation
    {
        // ─────────────────────────────────────────────────────────────
        //  One NavItem per group. Each setting lives in Subs (List<>).
        //  All RequiredPlan / IsNew / HasUpdate metadata preserved.
        // ─────────────────────────────────────────────────────────────
        public static readonly IReadOnlyList<NavItem> Groups = new List<NavItem>
        {
            new NavItem
            {
                Title = "General",
                Url   = "/Settings/General",
                Icon  = "fas fa-sliders-h",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Organisation Details", "/Settings/General/Organisation"),
                    Sub("Currency",             "/Settings/General/Currency"),
                    Sub("Service Locations",    "/Settings/General/ServiceLocations"),
                }
            },
            new NavItem
            {
                Title = "Account Management",
                Url   = "/Settings/Account",
                Icon  = "bi bi-person-circle",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Profile Details", "/Settings/Account/Profile"),
                    Sub("Preferences",  "/Settings/Account/Preferences"),
                    Sub("Notifications",  "/Settings/Account/Notifications"),
                    Sub("Security",    "/Settings/Account/Security"),
                }
            },

            new NavItem
            {
                Title = "Workforce",
                Url   = "/Settings/Workforce",
                Icon  = "fas fa-users",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Users",       "/Settings/Workforce/Users"),
                    Sub("Territories", "/Settings/Workforce/Territories"),
                    Sub("Holiday",     "/Settings/Workforce/Holiday"),
                    Sub("Skills",      "/Settings/Workforce/Skills", SubscriptionPlanEnum.Professional),
                    Sub("Shifts",      "/Settings/Workforce/Shifts", SubscriptionPlanEnum.Professional),
                }
            },

            new NavItem
            {
                Title = "Security Control",
                Url   = "/Settings/Security",
                Icon  = "fas fa-shield-alt",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Profiles",                "/Settings/Security/Profiles"),
                    Sub("Audit Log",               "/Settings/Security/AuditLog"),
                    Sub("PII Fields",              "/Settings/Security/PiiFields"),
                    Sub("Field Level Permissions", "/Settings/Security/FieldPermissions", SubscriptionPlanEnum.Professional),
                }
            },

            new NavItem
            {
                Title = "Process Flow",
                Url   = "/Settings/ProcessFlow",
                Icon  = "fas fa-project-diagram",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Transaction Rules", "/Settings/ProcessFlow/TransactionRules",
                        isNew: true, newText: "New"),
                }
            },

            new NavItem
            {
                Title = "Billing",
                Url   = "/Settings/Billing",
                Icon  = "fas fa-file-invoice-dollar",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Billing Setup", "/Settings/Billing/Setup"),
                    Sub("Tax Setting",   "/Settings/Billing/Tax"),
                    Sub("Sync Logs",     "/Settings/Billing/SyncLogs"),
                }
            },

            new NavItem
            {
                Title = "Customization",
                Url   = "/Settings/Customization",
                Icon  = "fas fa-puzzle-piece",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Modules and Fields", "/Settings/Customization/ModulesAndFields"),
                    Sub("Module Mapping",     "/Settings/Customization/ModuleMapping"),
                    Sub("Dispatch Console",   "/Settings/Customization/DispatchConsole"),
                    Sub("Templates",          "/Settings/Customization/Templates"),
                    Sub("Job Sheets",         "/Settings/Customization/JobSheets",  SubscriptionPlanEnum.Professional),
                    Sub("Dashboards",         "/Settings/Customization/Dashboards", SubscriptionPlanEnum.Professional),
                }
            },

            new NavItem
            {
                Title = "Field Service Settings",
                Url   = "/Settings/FieldService",
                Icon  = "fas fa-wrench",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Transaction Settings", "/Settings/FieldService/TransactionSettings",
                        hasUpdate: true, updateText: "New updates"),
                    Sub("Record Templates",     "/Settings/FieldService/RecordTemplates"),
                    Sub("Job Costing",          "/Settings/FieldService/JobCosting"),
                }
            },

            new NavItem
            {
                Title = "Automation",
                Url   = "/Settings/Automation",
                Icon  = "fas fa-robot",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Workflow Rules", "/Settings/Automation/WorkflowRules"),
                    Sub("Actions",        "/Settings/Automation/Actions",
                        hasUpdate: true, updateText: "Updated"),
                    Sub("Time Based Rules", "/Settings/Automation/TimeBasedRules",
                        SubscriptionPlanEnum.Professional),
                    Sub("Scheduled Report", "/Settings/Automation/ScheduledReport",
                        SubscriptionPlanEnum.Professional, hasUpdate: true, updateText: "New"),
                }
            },

            new NavItem
            {
                Title = "Data Administration",
                Url   = "/Settings/Data",
                Icon  = "fas fa-database",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Data Export",  "/Settings/Data/Export"),
                    Sub("Data Import",  "/Settings/Data/Import",
                        SubscriptionPlanEnum.Starter, isNew: true, newText: "New"),
                    Sub("File Storage", "/Settings/Data/FileStorage"),
                }
            },

            new NavItem
            {
                Title = "Developer Space",
                Url   = "/Settings/Developer",
                Icon  = "fas fa-code",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("APIs",                 "/Settings/Developer/Apis",
                        SubscriptionPlanEnum.Professional),
                    Sub("Email Usage",          "/Settings/Developer/EmailUsage"),
                    Sub("Standalone Functions", "/Settings/Developer/Functions",
                        SubscriptionPlanEnum.Enterprise),
                    Sub("Connections",          "/Settings/Developer/Connections",
                        SubscriptionPlanEnum.Professional, hasUpdate: true, updateText: "Updated"),
                }
            },

            new NavItem
            {
                Title = "Zoho Integration",
                Url   = "/Settings/Zoho",
                Icon  = "fas fa-plug",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Zoho Desk",      "/Settings/Zoho/Desk"),
                    Sub("Zoho Invoice",   "/Settings/Zoho/Invoice",   SubscriptionPlanEnum.Starter),
                    Sub("Zoho Flow",      "/Settings/Zoho/Flow",      SubscriptionPlanEnum.Starter),
                    Sub("Zoho CRM",       "/Settings/Zoho/Crm",       SubscriptionPlanEnum.Professional),
                    Sub("Bigin",          "/Settings/Zoho/Bigin",     SubscriptionPlanEnum.Professional),
                    Sub("Zoho Analytics", "/Settings/Zoho/Analytics", SubscriptionPlanEnum.Enterprise, isNew: true, newText: "New"),
                }
            },

            new NavItem
            {
                Title = "Third Party Integration",
                Url   = "/Settings/Integrations",
                Icon  = "fas fa-exchange-alt",
                Roles = new[] { "SuperAdmin" },
                Subs  = new List<NavSubItem>
                {
                    Sub("Zapier", "/Settings/Integrations/Zapier", SubscriptionPlanEnum.Starter),
                }
            },
        };

        // ─────────────────────────────────────────────────────────────
        //  Backwards-compatible single "Settings" root.
        //  Flattens all Subs from every group so existing code
        //  referencing Navigation.Settings still compiles.
        // ─────────────────────────────────────────────────────────────
        public static readonly NavItem Settings = new NavItem
        {
            Title = "Settings",
            Url = "/Settings",
            Icon = "fas fa-cogs",
            Roles = new[] { "SuperAdmin" },
            Subs = Groups.SelectMany(g => g.Subs).ToList()
        };

        // ─────────────────────────────────────────────────────────────
        //  Helper — keeps one line per setting.
        // ─────────────────────────────────────────────────────────────
        private static NavSubItem Sub(
            string title,
            string url,
            SubscriptionPlanEnum requiredPlan = SubscriptionPlanEnum.Free,
            bool isNew = false,
            string newText = "",
            bool hasUpdate = false,
            string updateText = "")
            => new NavSubItem
            {
                Title = title,
                Url = url,
                Roles = new[] { "SuperAdmin" },
                RequiredPlan = requiredPlan,
                IsNew = isNew,
                NewText = newText,
                HasUpdate = hasUpdate,
                UpdateText = updateText,
            };
    }
}