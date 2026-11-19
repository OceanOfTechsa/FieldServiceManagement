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
                Icon = "UserCog",
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
                Icon = "GitBranch",
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
                Icon = "Castle",
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
                Icon = "CircleDollarSign",
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
                Icon = "Flag",
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
                Title = "Announcements",
                Url = "",
                Icon = "Megaphone",
                Roles = ["SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "All Announcements",
                        Url = "/Admin/Announcements/",
                        Roles = ["SuperAdmin"]
                    }, 
                    new NavSubItem
                    {
                        Title = "Create Announcement",
                        Url = "/Admin/Announcements/Create",
                        Roles = ["SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Announcements Settings",
                        Url = "/Admin/Announcements/Settings",
                        Roles = ["SuperAdmin"]
                    },
                ]
            },
            new NavItem
            {
                Title = "Reference Data",
                Url = "",
                Icon = "Tangent",
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
                Icon = "FileInput",
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
                Icon = "Settings",
                Roles = ["SuperAdmin"],
               Subs =
               [
                   new NavSubItem
                   {
                       Title = "Create Settings",
                       Url = "/Admin/AppSettings/Create",
                       Roles = ["SuperAdmin"]
                   },
                    new NavSubItem
                   {
                       Title = "View Settings",
                       Url = "/Admin/AppSettings",
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
                Icon = "Send",
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
                Icon = "UserRoundGroup",
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
                Icon = "DiagramFlowVertical",
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
                Icon = "GraphHub",
                Roles = ["Administrator", "Dispatcher"],
                Subs = []
            },
            new NavItem
            {
                Title = "Billing",
                Url = "#",
                Icon = "CreditCardCheck",
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
                Icon = "WrenchOff",
                Roles = ["Administrator", "Dispatcher", "FieldAgent"],
                Subs = []
            },
            new NavItem
            {
                Title = "Workforce",
                Url = "#",
                Icon = "UsersRoundCollaboration",
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
                Icon = "ClipboardMinus",
                Roles = ["Administrator", "Dispatcher"],
                Subs = []
            },
            new NavItem
            {
                Title = "Addresses",
                Url = "/Addresses",
                Icon = "MapPinHouse",
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
        ];
    }
}