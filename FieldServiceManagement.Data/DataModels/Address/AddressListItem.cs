using FieldServiceManagement.Data.DataModels.BaseClass;
using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Address
{
    [NotMapped]
    public class AddressListItem : BaseGuidPrimaryKey
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedByAvatar { get; set; }
        public Guid? CreatedById { get; set; }
    }
}
