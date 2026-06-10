using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.WorkflowStageDefinition
{
    [Table("WorkflowStageDefinitions")]
    public class WorkflowStageDefinition : BaseIntPrimaryKey
    {
        public int FormId { get; set; }
        public int? StageOrder { get; set; }
        public string StageName { get; set; }
        public int? StatusOnEnter { get; set; }
        public int? StatusOnPass { get; set; }
        public int? StatusOnReject { get; set; }
        public int? RequiredApprovals { get; set; }
        public bool? AnyRejectFails { get; set; }
        public bool? AllowSelfApproval { get; set; }
        public bool? IsConditional { get; set; }
        public string ConditionType { get; set; }
        public int Version { get; set; }
        public bool IsCurrent { get; set; }
    }
}