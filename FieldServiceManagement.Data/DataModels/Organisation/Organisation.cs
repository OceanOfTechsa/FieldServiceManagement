using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Organisation
{
    [Table("Organisations")]
    public class Organisation : BaseGuidPrimaryKey
    {
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public int? IndustryId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CurrencyId { get; set; }
        public int? TimezoneId { get; set; }
        public int? LanguageId { get; set; }
        public int? PlanId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}