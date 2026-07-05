using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Announcement
{
    public class AnnouncementAdminListItemViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string VisibleToRoleIds { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
