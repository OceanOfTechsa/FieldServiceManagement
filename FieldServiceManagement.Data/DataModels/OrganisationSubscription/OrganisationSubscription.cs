using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.OrganisationSubscription
{
    [Table("OrganisationSubscriptions")]
    public class OrganisationSubscription : BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public int PlanId { get; set; }
        public string BillingCycle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancellationNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}