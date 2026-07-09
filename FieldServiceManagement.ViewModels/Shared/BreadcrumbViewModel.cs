namespace FieldServiceManagement.ViewModels.Shared
{
    public class BreadcrumbViewModel
    {
        /// <summary>Show the back chevron button before Home.</summary>
        public bool ShowBack { get; set; } = false;
        public bool ShowHome { get; set; } = true;

        /// <summary>Crumb items after Home. Last item with no Url renders as current page.</summary>
        public List<BreadcrumbItem> Items { get; set; } = new();
        public string Icon { get; set; } = "bi-folder2";
    }

    public class BreadcrumbItem
    {
        public string Label { get; set; } = string.Empty;

        /// <summary>Null or empty = current page (no link rendered).</summary>
        public string? Url { get; set; }
    }
}