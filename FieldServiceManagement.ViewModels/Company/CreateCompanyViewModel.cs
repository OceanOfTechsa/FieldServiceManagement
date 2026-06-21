using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Company
{
    public class CreateCompanyViewModel : IHasOrganisationAddresses
    {
        [Required(ErrorMessage = "Please provide a valid company name")]
        public string Name { get; set; }

        [Url]
        public string Website { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please Select Service Address")]
        [DisplayName("Company Type")]
        public int Type { get; set; }

        [Required(ErrorMessage = "Please Select Service Address")]
        [DisplayName("Service Address")]
        public Guid ServiceAddressId { get; set; }

        [Required(ErrorMessage = "Please Select Billinng Address")]
        [DisplayName("Billing Address")]
        public Guid BillingAddressId { get; set; }

        public List<AddressViewModel>? OrganisationAddresses { get; set; }

        public Guid? OrganisationId { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
