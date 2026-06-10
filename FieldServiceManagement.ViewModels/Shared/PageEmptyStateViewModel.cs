using FieldServiceManagement.Enum;

namespace FieldServiceManagement.ViewModels.Shared
{
    public class PageEmptyStateViewModel
    {
        public string PageTitle { get; set; } = string.Empty;
        public string PageContent { get; set; } = string.Empty;
        public PublicHelpEnum HelpEnum { get; set; }
        public string? ImageUrl { get; set; }
        public List<EmptyStateAction> Actions { get; set; } = new();
    }
}
