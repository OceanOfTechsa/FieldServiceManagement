using FieldServiceManagement.Data.DataModels.BaseClass;
using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Interfaces;
using System.ComponentModel;

namespace FieldServiceManagement.ViewModels.Company
{
    public class CompanyViewModel: BaseGuidPrimaryKeyViewModel, IHasOrganisationAddresses
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Mobile { get; set; }
        public string? Website { get; set; }
        public int Type { get; set; }

        [DisplayName("Service Address")]
        public Guid ServiceAddressId { get; set; }

        [DisplayName("Billing Address")]
        public Guid BillingAddressId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public Guid OrganisationId { get; set; }

        public List<AddressViewModel>? OrganisationAddresses { get; set; }
    }
}
