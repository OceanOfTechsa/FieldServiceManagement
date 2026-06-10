using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.WorkflowApproverAssignment
{
    [Table("WorkflowApproverAssignments")]
    public class WorkflowApproverAssignment : BaseGuidPrimaryKey
    {
        public int StageDefinitionId { get; set; }
        public Guid? UserId { get; set; }
        public int? UserRole { get; set; }
        public string RoleName { get; set; }
        public Guid? DepartmentId { get; set; }
        public bool? AllowMultiple { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}