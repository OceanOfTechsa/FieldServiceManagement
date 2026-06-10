using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Payment
{
    [Table("Payments")]
    public class Payment : BaseGuidPrimaryKey
    {
        public Guid InvoiceId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PaidAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}