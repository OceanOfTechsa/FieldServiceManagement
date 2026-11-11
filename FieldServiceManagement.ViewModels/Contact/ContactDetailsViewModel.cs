using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.Company;
using FieldServiceManagement.ViewModels.Organisation;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.ViewModels.Contact
{
    public class ContactDetailsViewModel
    {
        public ContactViewModel Contact { get; set; }
        public List<AuditLogViewModel>? AuditLogs { get; set; }
        public AppUserViewModel? CreatedBy { get; set; }
        public AppUserViewModel? UpdatedBy { get; set; }
        public AppUserViewModel? Owner { get; set; }
        public OrganisationViewModel Organisaion { get; set; }
        public CompanyViewModel Company { get; set; }
        public List<EntityLinkedAddressViewModel> LinkedAddresses { get; set; } = new();
        public List<AddressViewModel>? OrganisationAddresses { get; set; }
    }
}
