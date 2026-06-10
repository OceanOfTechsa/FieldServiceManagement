using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Company;
using FieldServiceManagement.ViewModels.Interfaces;
using System.ComponentModel;

namespace FieldServiceManagement.ViewModels.Contact
{
    public class CreateContactViewModel : IHasOrganisationAddresses
    {
        public int Salutation { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        [DisplayName("Company")]
        public Guid CompanyId { get; set; }

        [DisplayName("Service Address")]
        public Guid? ServiceAddressId { get; set; }

        [DisplayName("Billing Address")]
        public Guid? BillingAddressId { get; set; }

        public Guid? OrganisationId { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public List<AddressViewModel>? OrganisationAddresses { get; set; }
        public IEnumerable<CompanyListItemViewModel>? Companies { get; set; }
    }
}
