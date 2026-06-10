using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Address
{
    [Table("Addresses")]
    public class Address : BaseGuidPrimaryKey
    {
        public Guid OrganisationId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
    }
}