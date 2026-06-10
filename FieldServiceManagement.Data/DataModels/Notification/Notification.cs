using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Notification
{
    [Table("Notifications")]
    public class Notification : BaseGuidPrimaryKey
    {
        public Guid? OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedById { get; set; }
    }
}