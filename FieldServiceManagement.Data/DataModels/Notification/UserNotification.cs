using FieldServiceManagement.Data.DataModels.BaseClass;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Data.DataModels.Notification
{
    [Keyless]
    public class UserNotification : BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public string? RelatedEntityName { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public byte Severity { get; set; }
        public string? SeverityCode { get; set; }
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
    }
}