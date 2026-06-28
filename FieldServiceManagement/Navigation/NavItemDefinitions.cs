using FieldServiceManagement.Models;

namespace FieldServiceManagement.Web.Navigation
{
    public static class NavItemDefinitions
    {
        public static readonly List<NavItem> Items =
        [
            new NavItem
            {
                Title = "Form Submissions",
                Url = "/Formsubmission",
                Icon = "fa-solid fa-paper-plane",
                Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent", "SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Pending",
                        Url = "/Formsubmission/Pending",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "CallCenterAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "History",
                        Url = "/Formsubmission/History",
                        Roles = ["Administrator", "Dispatcher", "CallCenterAgent", "SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Workflow",
                Url = "/Workflow",
                Icon = "fa-solid fa-chart-diagram",
                Roles = ["Administrator", "SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Config",
                        Url = "/Workflow/Config",
                        Roles = ["Administrator", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "More",
                        Url = "/Workflow/more",
                        Roles = ["Administrator", "SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Customers",
                Url = "#",
                Icon = "fa-solid fa-wand-magic-sparkles",
                Roles = ["Administrator", "Dispatcher", "CallCenterAgent", "SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Contacts",
                        Url = "/Customers/Contacts",
                        Roles = ["Administrator", "Dispatcher", "CallCenterAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Companies",
                        Url = "/Customers/Companies",
                        Roles = ["Administrator", "Dispatcher", "CallCenterAgent", "SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Work Order Management",
                Url = "#",
                Icon = "fa-solid fa-code-branch",
                Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent", "SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Requests",
                        Url = "/WorkOrderManagement/Requests",
                        Roles = ["Administrator", "Dispatcher", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Estimates",
                        Url = "/WorkOrderManagement/Estimates",
                        Roles = ["Administrator", "Dispatcher", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Work Orders",
                        Url = "/WorkOrderManagement/WorkOrders",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Service Appointments",
                        Url = "/WorkOrderManagement/ServiceAppointments",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Service Reports",
                        Url = "/WorkOrderManagement/ServiceReports",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "LimitedFieldAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Scheduled Maintenance",
                        Url = "/WorkOrderManagement/ScheduledMaintenances",
                        Roles = ["Administrator", "Dispatcher", "SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Dispatch Console",
                Url = "/DispatchConsole",
                Icon = "fa-solid fa-truck-arrow-right",
                Roles = ["Administrator", "Dispatcher", "SuperAdmin"],
                Subs = []
            },
            new NavItem
            {
                Title = "Billing",
                Url = "#",
                Icon = "fa fa-landmark",
                Roles = ["Administrator", "Dispatcher", "SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Invoices",
                        Url = "/Billing/Invoices",
                        Roles = ["Administrator", "Dispatcher", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Payments",
                        Url = "/Billing/Payments",
                        Roles = ["Administrator", "Dispatcher", "SuperAdmin"]
                    }
                ]
            },
            new NavItem
            {
                Title = "Services & Parts",
                Url = "/ServicesAndParts",
                Icon = "fa-solid fa-hand-holding-hand",
                Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"],
                Subs = []
            },
            new NavItem
            {
                Title = "Workforce",
                Url = "#",
                Icon = "fa-solid fa-users",
                Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"],
                Subs =
                [
                    new NavSubItem
                    {
                        Title = "Users",
                        Url = "/Workforce/Users",
                        Roles = ["Administrator", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Crew",
                        Url = "/Workforce/Crew",
                        Roles = ["Administrator", "Dispatcher", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Equipment",
                        Url = "/Workforce/Equipment",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Attendance",
                        Url = "/Workforce/Attendance",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Time Off",
                        Url = "/Workforce/TimeOff",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Trips",
                        Url = "/Workforce/Trips",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"]
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
                Roles = ["Administrator", "Dispatcher", "CallCenterAgent"],
                Subs = 
                [
                    new NavSubItem
                    {
                        Title = "All Addresses",
                        Url = "/Addresses",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"]
                    },
                    new NavSubItem
                    {
                        Title = "Create",
                        Url = "/Addresses/Create",
                        Roles = ["Administrator", "Dispatcher", "FieldAgent", "SuperAdmin"]
                    }
                ]
            }
        ];
    }
}