namespace FieldServiceManagement.ViewModels.Announcement
{
    public class UserAnnouncementViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsSeen { get; set; }
        public DateTime? SeenDate { get; set; }

        public string? VisibleToRoleIds { get; set; }
        public int StatusId { get; set; }

        // Computed for display — "45 min ago" style
        public string TimeAgoDisplay => GetTimeAgoDisplay(AnnouncementDate);

        private static string GetTimeAgoDisplay(DateTime date)
        {
            var span = DateTime.UtcNow - date;

            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} min ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hr ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays} day{((int)span.TotalDays == 1 ? "" : "s")} ago";

            return date.ToLocalTime().ToString("dd MMM yyyy");
        }
    }
}