using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.UserProfileAudit
{
    [Table("UserProfileAudits")]
    public class UserProfileAudit : BaseGuidPrimaryKey
    {
        public DateTime CreatedAt { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid UserId { get; set; }
        public Guid PerformByUserId { get; set; }
        public string PerformedByName { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
        public string IpAddress { get; set; }
        public string? FieldName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public bool IsDeleted { get; set; }
    }
}