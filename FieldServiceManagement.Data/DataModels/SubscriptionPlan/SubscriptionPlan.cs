using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.SubscriptionPlan
{
    [Table("SubscriptionPlans")]
    public class SubscriptionPlan : BaseIntPrimaryKey
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? MaxUsers { get; set; }
        public int? MaxWorkOrders { get; set; }
        public int? MaxForms { get; set; }
        public int? MaxStorageMb { get; set; }
        public decimal? MonthlyPrice { get; set; }
        public decimal? AnnualPrice { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}