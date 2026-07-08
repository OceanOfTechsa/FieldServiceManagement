using FieldServiceManagement.Data.DataModels.BaseClass;

namespace FieldServiceManagement.ViewModels.Notification
{
    public class UserNotificationViewModel : BaseGuidPrimaryKeyViewModel
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RelatedEntityType { get; set; }
        public string RelatedEntityId { get; set; }
        public string RelatedEntityName { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte Severity { get; set; }
        public string SeverityCode { get; set; }
        public string ActionText { get; set; }
        public string ActionUrl { get; set; }
    }
}
