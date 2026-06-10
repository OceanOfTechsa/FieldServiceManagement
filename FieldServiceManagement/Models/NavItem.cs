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
    }
}