using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.FormSubmissionAudit
{
    [Table("FormSubmissionAudits")]
    public class FormSubmissionAudit : BaseGuidPrimaryKey
    {
        public DateTime CreatedAt { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid PerformByUserId { get; set; }
        public Guid FormSubmissionId { get; set; }
        public int? StatusId { get; set; }
        public string Comment { get; set; }
        public string VisibleTo { get; set; }
        public int? DisclaimerId { get; set; }
        public bool IsDeleted { get; set; }
    }
}