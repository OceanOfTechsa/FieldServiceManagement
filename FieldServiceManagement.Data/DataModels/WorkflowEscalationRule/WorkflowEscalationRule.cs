using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.WorkflowEscalationRule
{
    [Table("WorkflowEscalationRules")]
    public class WorkflowEscalationRule : BaseGuidPrimaryKey
    {
        public int StageDefinitionId { get; set; }
        public int EscalationAfterHours { get; set; }
        public int? EscalateToRole { get; set; }
        public Guid? EscalateToUserId { get; set; }
        public string EscalationReason { get; set; }
        public int? EscalationLevel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}