using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.FormSubmissionStageDecision
{
    [Table("FormSubmissionStageDecisions")]
    public class FormSubmissionStageDecision : BaseGuidPrimaryKey
    {
        public Guid FormSubmissionId { get; set; }
        public int StageDefinitionId { get; set; }
        public Guid? DecidedByUserId { get; set; }
        public DateTime? DecidedAt { get; set; }
        public int? StatusId { get; set; }
        public string Comment { get; set; }
        public Guid? ApprovalProgressId { get; set; }
        public string DecidedRoleName { get; set; }
        public string PayloadJson { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}