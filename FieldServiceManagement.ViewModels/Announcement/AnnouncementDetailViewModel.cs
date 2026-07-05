namespace FieldServiceManagement.ViewModels.Announcement
{
    public class AnnouncementDetailViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int StatusId { get; set; }
        public string? VisibleToRoleIds { get; set; }

        // Created by
        public Guid? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedBySurname { get; set; }
        public string? CreatedByAvatar { get; set; }

        // Updated by
        public Guid? UpdatedById { get; set; }
        public string? UpdatedByName { get; set; }
        public string? UpdatedBySurname { get; set; }
        public string? UpdatedByAvatar { get; set; }

        // Computed
        public string CreatedByFullName =>
            new[] { CreatedByName, CreatedBySurname }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Aggregate((a, b) => $"{a} {b}") ?? "Unknown";

        public string UpdatedByFullName =>
            new[] { UpdatedByName, UpdatedBySurname }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Aggregate((a, b) => $"{a} {b}") ?? "—";

        public string StatusLabel => StatusId switch
        {
            27 => "Published",
            26 => "Draft",
            28 => "Archived",
            _ => "Draft"
        };

        public string StatusCss => StatusId switch
        {
            27 => "bg-success",
            28 => "bg-warning",
            _ => "bg-secondary"
        };

        public List<string> VisibleToRoleList =>
            string.IsNullOrWhiteSpace(VisibleToRoleIds)
                ? new List<string> { "All Roles" }
                : VisibleToRoleIds.Split(',')
                    .Select(id => id.Trim())
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Select(id => RoleDisplayName(id))
                    .ToList();

        private static string RoleDisplayName(string id) => id switch
        {
            "1" => "Super Admin",
            "2" => "Administrator",
            "3" => "Manager",
            "4" => "Call Centre Agent",
            "5" => "Field Technician",
            "6" => "Client",
            "7" => "Viewer",
            _ => $"Role {id}"
        };
    }
}