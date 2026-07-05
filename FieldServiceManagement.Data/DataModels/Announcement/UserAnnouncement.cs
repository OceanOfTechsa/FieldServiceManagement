using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.Data.DataModels.Announcement
{
    public class UserAnnouncement : BaseGuidPrimaryKey
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Notes { get; set; }
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
    }
}
