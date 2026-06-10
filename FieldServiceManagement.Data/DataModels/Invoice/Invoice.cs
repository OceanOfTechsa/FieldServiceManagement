using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Invoice
{
    [Table("Invoices")]
    public class Invoice : BaseGuidPrimaryKey
    {
        public Guid WorkOrderId { get; set; }
        public decimal? Amount { get; set; }
        public int? StatusId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}