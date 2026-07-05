using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Data.DataModels.Announcement
{
    [Keyless]
    public class AnnouncementDetail
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
    }
}