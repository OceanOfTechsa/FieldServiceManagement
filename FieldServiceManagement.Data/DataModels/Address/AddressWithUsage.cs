using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Address
{
    [NotMapped]
    public class AddressWithUsage : BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public string Street { get; set; } = string.Empty;
        public string? City { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? ZipCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }

        public int UsageCount { get; set; }
        public string? UsedBy { get; set; }
    }
}
