using System.ComponentModel.DataAnnotations.Schema;

namespace FieldServiceManagement.Data.DataModels.Address
{
    [NotMapped]
    public class CreateAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Guid CreatedById { get; set; }
        public Guid OrganisationId { get; set; }
        public string? IpAddress { get; set; }
    }
}
