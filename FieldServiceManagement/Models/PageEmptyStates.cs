using FieldServiceManagement.Enum;
using FieldServiceManagement.ViewModels.Shared;

namespace FieldServiceManagement.Models
{
    public static class PageEmptyStates
    {
        public static PageEmptyStateViewModel Companies => new()
        {
            PageTitle = "Companies",
            PageContent = "Simplify the creation and management of customer service requests to ensure timely responses and high-quality service. Easily capture key details such as contact information, service requirements, and preferences via a user-friendly web form or manually from emails or phone calls.",
            HelpEnum = PublicHelpEnum.Companies,
            ImageUrl = "/Assets/Images/Application/companies.svg",
            Actions =
                [
                    new(){Label = "Create Company",Url="/Customers/Companies/Create",CssClass = "btn btn-sm btn-primary"},
                    new(){ Label = "Import Data", Action = "Import",  Controller = "Companies", IsPrimary = false, CssClass = "btn border-fsm btn-xs fs-6" }
                ]
        };

        public static PageEmptyStateViewModel Contacts => new()
        {
            PageTitle = "Contacts",
            PageContent = "Manage your contacts in one place. Track individuals across companies, log communication history, and keep service relationships organised and accessible.",
            HelpEnum = PublicHelpEnum.Contacts,
            ImageUrl = "/Assets/Images/Application/contacts.svg",
            Actions =
            [
                new() { Label = "Create Contact", Url="/Customers/Contacts/Create", CssClass="btn btn-sm btn-primary" },
                new() { Label = "Import Data", Action = "Import",  Controller = "Contacts", IsPrimary = false, CssClass = "btn border-fsm btn-xs fs-6" }
            ]
        };

        public static PageEmptyStateViewModel WorkOrders => new()
        {
            PageTitle = "Work Orders",
            PageContent = "Create and dispatch work orders to field agents. Monitor progress in real time, attach documents, and close jobs with full audit trails.",
            HelpEnum = PublicHelpEnum.WorkOrders,
            ImageUrl = "/Assets/Images/Application/work-orders.svg",
            Actions =
            [
                new() { Label = "Create Work Order", Url="/WorkOrderManagement/Create", Icon = "ti ti-clipboard-plus" }
            ]
        };

        public static PageEmptyStateViewModel Crew => new()
        {
            PageTitle = "Crew",
            PageContent = "Crew refers to a group of field technicians who are assigned to work together on specific work orders. Crews are typically created when a job requires more than one person with different set of skills for jobs such as large-scale installations, complex repairs. It is possible to track the activities, time spent, and performance of crews, just as it is done for individual technicians.",
            HelpEnum = PublicHelpEnum.Crew,
            ImageUrl = "/Assets/Images/Application/crews.svg",
            Actions =
            [
                new() { Label = "Create Crew", Url="/Workforce/Crew/Create", Icon = "bi-people-fill" }
            ]
        };
        // Add more pages here following the same pattern...
    }
}
