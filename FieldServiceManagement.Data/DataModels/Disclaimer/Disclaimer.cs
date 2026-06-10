using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Disclaimer
{
    [Table("Disclaimers")]
    public class Disclaimer : BaseIntPrimaryKey
    {
        public DateTime? DisclaimerStartDate { get; set; }
        public DateTime? DisclaimerEndDate { get; set; }
        public string DisclaimerDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}