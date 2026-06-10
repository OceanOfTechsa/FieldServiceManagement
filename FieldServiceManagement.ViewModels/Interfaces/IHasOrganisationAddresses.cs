using FieldServiceManagement.ViewModels.Address;
namespace FieldServiceManagement.ViewModels.Interfaces
{
    public interface IHasOrganisationAddresses
    {
        List<AddressViewModel>? OrganisationAddresses { get; set; }
    }
}
