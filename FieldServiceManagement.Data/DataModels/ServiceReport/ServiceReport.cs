using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.ServiceReport
{
    [Table("ServiceReports")]
    public class ServiceReport : BaseGuidPrimaryKey
    {
        public Guid WorkOrderId { get; set; }
        public string ReportText { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}