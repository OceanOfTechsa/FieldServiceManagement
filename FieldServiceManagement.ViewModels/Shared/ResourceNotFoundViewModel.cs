namespace FieldServiceManagement.ViewModels.Shared
{
    public class ResourceNotFoundViewModel
    {
        public string ResourceName { get; set; } = "resource";

        public string? ReturnUrl { get; set; }

        public string Title { get; set; } = "Resource Not Found";
    }
}
