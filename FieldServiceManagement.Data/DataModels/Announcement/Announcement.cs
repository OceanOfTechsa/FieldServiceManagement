using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Announcement
{
    [Table("Announcements")]
    public class Announcement : BaseGuidPrimaryKey
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

        public string? VisibleToRoleIds { get; set; }
        public int StatusId { get; set; }
    }
}
