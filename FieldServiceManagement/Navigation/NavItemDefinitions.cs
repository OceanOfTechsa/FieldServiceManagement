using FieldServiceManagement.Models;

namespace FieldServiceManagement.Web.Navigation
{
    public static class NavItemDefinitions
    {
        public static readonly List<NavItem> Items =
        [
            #region SUPER ADMIN ROUTES
            new NavItem
            {
                Title = "Admin Dashboard",
                Url = "/Admin",
                Icon = "bi bi-person-fill-gear",
                Roles = ["SuperAdmin"],
               Subs =
               [
                   new NavSubItem
                   {
                       Title = "System Health",
                       Url = "/Admin/Health/",
                       Roles = ["SuperAdmin"]
                   },
                   new NavSubItem
                   {
                       Title = "Pending Alerts",
                       Url = "/Admin/PendingAlerts",
                       Roles = ["SuperAdmin"]
                   },
                    new NavSubItem
                   {
                       Title = "View All Alerts",
                       Url = "/Admin/ViewAllAlerts",
                       Roles = ["SuperAdmin"]
                   },
                   new NavSubItem
                   {
                       Title = "Resend Email",
                       Url = "/Admin/ResendEmail",
                       Roles = ["SuperAdmin"]
                   },
                   new NavSubItem
                   {
                       Title = "SQL Firewall",
                       Url = "/Admin/SQLFirewall",
                       Roles = ["SuperAdmin"]
                   },
                   new NavSubItem
                   {
                       Title = "Flush Cache",
                       Url = "/Admin/FlushCache",
                       Roles = ["SuperAdmin"]
                   },
                   //new NavSubItem
                   //{
                   //    Title = "Rebuild search indexes",
                   //    Url = "/Admin/RebuildSearchIndexes",
                   //    Roles = ["SuperAdmin"]
                   //},
                   //new NavSubItem
                   //{
                   //    Title = "View storage usage",
                   //    Url = "/Admin/ViewStorageUsage",
                   //    Roles = ["SuperAdmin"]
                   //},
               ]
            },
            new NavItem
            {
                Title = "Workflow",
                Url = "/Workflow",
                Icon = "fa-solid fa-chart-diagram",
                Roles = ["SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "WorkFlow Stages",
                        Url = "/Admin/Workflow/Stages",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Workflow Approvers",
                        Url = "/Admin/Workflow/Approvers",
                        Roles = ["SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Organisations",
                Url = "/Admin/Organisations",
                Icon = "fa-solid fa-building",
                Roles = ["SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "All Organisations",
                        Url = "/Admin/Organisations",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Health Status",
                        Url = "/Admin/Organisations/HealthStatus",
                        Roles = ["SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Subscriptions",
                Url = "/Admin/Subscriptions",
                Icon = "fa-solid fa-credit-card",
                Roles = ["SuperAdmin"],
            },
            new NavItem
            {
                Title = "User Management",
                Url = "/Admin/UserManagement",
                Icon = "fa-solid fa-users",
                Roles = ["SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Create User",
                        Url = "/Admin/Users/Create",
                        Roles = ["SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Features",
                Url = "/FeatureManagement",
                Icon = "bi bi-flag",
                Roles = ["SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "All Features",
                        Url = "/FeatureManagement/AllFeatures",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Assign To Organisation",
                        Url = "/FeatureManagement/AssignToOrganisation",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Assign To Subscription",
                        Url = "/FeatureManagement/AssignToSubscription",
                        Roles = ["SuperAdmin"]
                    },
                ]
            },
            new NavItem
            {
                Title = "Notifications",
                Url = "",
                Icon = "bi bi-megaphone-fill",
                Roles = ["SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Send Broadcast",
                        Url = "/Admin/Notifications/SendBroadcast",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Send Maintenance Notice",
                        Url = "/Admin/Notifications/SendMaintenanceNotice",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Schedule Maintenance",
                        Url = "/Admin/Notification/ScheduleMaintenance",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "View Notification History",
                        Url = "/Admin/Notifications/Histroty",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Timezones",
                        Url = "/Admin/Timezones",
                        Roles = ["SuperAdmin"]
                    },
                ]
            },
            new NavItem
            {
                Title = "Reference Data",
                Url = "",
                Icon = "bi bi-bezier",
                Roles = ["SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Countries",
                        Url = "/Admin/ReferenceData/Countries",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Provinces",
                        Url = "/Admin/ReferenceData/Provinces",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Currencies",
                        Url = "/Admin/ReferenceData/Currencies",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Languages",
                        Url = "/Languages",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Timezones",
                        Url = "/Admin/ReferenceData/Timezones",
                        Roles = ["SuperAdmin"]
                    },
                ]
            },
            new NavItem
            {
                Title = "Forms",
                Url = "/Admin",
                Icon = "bi bi-input-cursor-text",
                Roles = ["SuperAdmin"],
               Subs =
               [
                   new NavSubItem
                   {
                       Title = "Work Order",
                       Url = "/Admin/Forms/WorkOrder",
                       Roles = ["SuperAdmin"]
                   },
                    new NavSubItem
                   {
                       Title = "Leave Application",
                       Url = "/Admin/Forms/LeaveApplication",
                       Roles = ["SuperAdmin"]
                   },
                   new NavSubItem
                   {
                       Title = "Asset Application",
                       Url = "/Admin/Forms/AssetApplication",
                       Roles = ["SuperAdmin"]
                   },
               ]
            },
            new NavItem
            {
                Title = "Settings",
                Icon = "bi bi-gear-fill",
                Roles = ["SuperAdmin"],
               Subs =
               [
                   new NavSubItem
                   {
                       Title = "Create Settings",
                       Url = "Admin/Settings/Create",
                       Roles = ["SuperAdmin"]
                   },
                    new NavSubItem
                   {
                       Title = "View Settings",
                       Url = "Admin//Settings/AllSettings",
                       Roles = ["SuperAdmin"]
                   },
               ]
            },
            #endregion

            #region SYSTEM OPARATIONS ROUTES
            new NavItem
            {
                Title = "Form Submissions",
                Url = "/Formsubmission",
                Icon = "fa-solid fa-paper-plane",
                Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent", "CustomerPortalUser"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Pending",
                        Url = "/Formsubmission/Pending",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent", "CustomerPortalUser"]
                    },
                    new NavSubItem
                    {
                        Title = "History",
                        Url = "/Formsubmission/History",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent", "CustomerPortalUser"]
                    }
                ]
            },                             
            new NavItem
            {
                Title = "Customers",
                Url = "#",
                Icon = "fa-solid fa-wand-magic-sparkles",
                Roles = ["Administrator", "Dispatcher", "CallCenterAgent"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Contacts",
                        Url = "/Customers/Contacts",
                        Roles = ["Administrator", "Dispatcher", "CallCenterAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Companies",
                        Url = "/Customers/Companies",
                        Roles = ["Administrator", "Dispatcher", "CallCenterAgent"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Work Order Management",
                Url = "#",
                Icon = "fa-solid fa-code-branch",
                Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Requests",
                        Url = "/WorkOrderManagement/Requests",
                        Roles = ["Administrator", "Dispatcher"]
                    },
                    new NavSubItem
                    {
                        Title = "Estimates",
                        Url = "/WorkOrderManagement/Estimates",
                        Roles = ["Administrator", "Dispatcher"]
                    },
                    new NavSubItem
                    {
                        Title = "Work Orders",
                        Url = "/WorkOrderManagement/WorkOrders",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Service Appointments",
                        Url = "/WorkOrderManagement/ServiceAppointments",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Service Reports",
                        Url = "/WorkOrderManagement/ServiceReports",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Scheduled Maintenance",
                        Url = "/WorkOrderManagement/ScheduledMaintenances",
                        Roles = ["Administrator", "Dispatcher"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Dispatch Console",
                Url = "/DispatchConsole",
                Icon = "fa-solid fa-truck-arrow-right",
                Roles = ["Administrator", "Dispatcher"],
                Subs = []
            },
            new NavItem
            {
                Title = "Billing",
                Url = "#",
                Icon = "fa fa-landmark",
                Roles = ["Administrator", "Dispatcher"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Invoices",
                        Url = "/Billing/Invoices",
                        Roles = ["Administrator", "Dispatcher"]
                    },
                    new NavSubItem
                    {
                        Title = "Payments",
                        Url = "/Billing/Payments",
                        Roles = ["Administrator", "Dispatcher"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Services & Parts",
                Url = "/ServicesAndParts",
                Icon = "fa-solid fa-hand-holding-hand",
                Roles = ["Administrator", "Dispatcher", "FieldAgent"],
                Subs = []
            },
            new NavItem
            {
                Title = "Workforce",
                Url = "#",
                Icon = "fa-solid fa-users",
                Roles = ["Administrator", "Dispatcher", "FieldAgent"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Users",
                        Url = "/Workforce/Users",
                        Roles = ["Administrator", "Dispatcher"]
                    },
                    new NavSubItem
                    {
                        Title = "Crew",
                        Url = "/Workforce/Crew",
                        Roles = ["Administrator", "Dispatcher"]
                    },
                    new NavSubItem
                    {
                        Title = "Equipment",
                        Url = "/Workforce/Equipment",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Attendance",
                        Url = "/Workforce/Attendance",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Time Off",
                        Url = "/Workforce/TimeOff",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Trips",
                        Url = "/Workforce/Trips",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Reports",
                Url = "/Reports",
                Icon = "fa-solid fa-file-lines",
                Roles = ["Administrator", "Dispatcher"],
                Subs = []
            },
            new NavItem
            {
                Title = "Addresses",
                Url = "/Addresses",
                Icon = "fa-solid fa-file-lines",
                Roles = ["Administrator", "Dispatcher", "CallCenterAgent", "FieldAgent"],
                Subs = 
                [
                    new NavSubItem
                    {
                        Title = "All Addresses",
                        Url = "/Addresses",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent"]
                    },
                    new NavSubItem
                    {
                        Title = "Create",
                        Url = "/Addresses/Create",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent"]
                    }
                ]
            },
            #endregion

            #region CLIENT ROUTES
             new NavItem
            {
                Title = "Form Submissions ",
                Url = "/Formsubmission",
                Icon = "fa-solid fa-paper-plane",
                Roles = ["CustomerPortalUser"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Pending",
                        Url = "/Formsubmission/Pending",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent", "CustomerPortalUser"]
                    },
                    new NavSubItem
                    {
                        Title = "History",
                        Url = "/Formsubmission/History",
                        Roles = ["Administrator", "Dispatcher", "CallCenterAgent", "CustomerPortalUser"]
                    }
                ]
            },  
	        #endregion
        ];
    }
}