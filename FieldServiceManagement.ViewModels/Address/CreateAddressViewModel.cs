using FieldServiceManagement.ViewModels.Country;
using FieldServiceManagement.ViewModels.State;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Address
{
    public class CreateAddressViewModel
    {
        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [DisplayName("State/Province")]
        public int? StateId { get; set; }

        [Required]
        [DisplayName("Country")]
        public int? CountryId { get; set; }

        [Required]
        [DisplayName("Zip Code")]
        [Length(4, 4, ErrorMessage = "Zip code must be 4 numbers")]
        public string ZipCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public List<CountryViewModel>? Countries { get; set; }
        public List<StateViewModel>? States { get; set; }

        public string? ReturnUrl { get; set; }


        public Guid? OrganisationId { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public string? IpAddress { get; set; }
    }
}
