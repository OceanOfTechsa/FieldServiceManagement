using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.FormApprovalProgress
{
    [Table("FormApprovalProgress")]
    public class FormApprovalProgress : BaseGuidPrimaryKey
    {
        public Guid FormSubmissionId { get; set; }
        public Guid? UserId { get; set; }
        public int AlertCount { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool? IsSecondaryManager { get; set; }
        public bool? IsMuted { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}