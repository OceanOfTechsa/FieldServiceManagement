using FieldServiceManagement.Data.DataModels.BaseClass;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Data.DataModels.Announcement
{
    [Keyless]
    public class AnnouncementAdminListItem : BaseGuidPrimaryKey
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Notes { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string? VisibleToRoleIds { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public Guid? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}