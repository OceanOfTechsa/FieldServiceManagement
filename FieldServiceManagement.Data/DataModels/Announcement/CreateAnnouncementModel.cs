using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.Data.DataModels.Announcement
{
    public class CreateAnnouncementModel : BaseGuidPrimaryKey
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public int StatusId { get; set; }
        public bool IsActive { get; set; }
        public string VisibleToRoleIds { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByEmail { get; set; }
    }
}