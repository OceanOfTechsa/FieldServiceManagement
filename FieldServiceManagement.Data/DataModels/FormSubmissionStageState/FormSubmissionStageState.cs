using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.FormSubmissionStageState
{
    [Table("FormSubmissionStageStates")]
    public class FormSubmissionStageState : BaseGuidPrimaryKey
    {
        public Guid FormSubmissionId { get; set; }
        public int StageDefinitionId { get; set; }
        public DateTime StageAssignedAt { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}