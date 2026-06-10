namespace FieldServiceManagement.ViewModels.Shared
{
    public enum ActionType
    {
        Redirect =1,
        ElementTrigger = 2
    }
    public class EmptyStateAction
    {
        public string Label { get; set; } = string.Empty;

        public string? Url { get; set; }

        public string? OnClick { get; set; }

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string CssClass { get; set; } = "btn btn-primary btn-sm d-flex gap-2";

        public string? Icon { get; set; }

        public bool IsPrimary { get; set; } = true;

        public ActionType? ActionType { get; set; }

        public string? TriggerElement { get; set; }
    }
}
