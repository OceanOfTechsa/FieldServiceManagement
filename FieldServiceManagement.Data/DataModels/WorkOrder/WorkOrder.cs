using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.WorkOrder
{
    [Table("WorkOrders")]
    public class WorkOrder : BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public Guid? FormSubmissionId { get; set; }
        public Guid? CustomerId { get; set; }
        public string Summary { get; set; }
        public int? Priority { get; set; }
        public int? StatusId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}