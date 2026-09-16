using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.Contact;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.ViewModels.Company
{
    public class CompanyDetailsViewModel
    {
        public CompanyViewModel? Company { get; set; }
        public List<AuditLogViewModel>? AuditLogs { get; set; }
        public AppUserViewModel? CreatedBy { get; set; }
        public AppUserViewModel? UpdatedBy { get; set; }
        public AppUserViewModel? Owner { get; set; }
        public List<EntityLinkedAddressViewModel> LinkedAddresses { get; set; } = new();
        public List<ContactViewModel> Contacts { get; set; } = new();
        public List<AddressViewModel>? OrganisationAddresses { get; set; }
    }
}