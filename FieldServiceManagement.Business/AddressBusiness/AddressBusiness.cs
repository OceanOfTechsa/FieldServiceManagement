using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository;
using FieldServiceManagement.ViewModels.Address;

namespace FieldServiceManagement.Business.AddressBusiness
{
    public class AddressBusiness
    {

        public async Task<List<AddressViewModel>> GetAllAddressesByOrganisationIdAsync(string Email)
        {
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(Email);
            var addresses = new AddressRepository().GetAllAddressesByOrganisaId(currentUser.OrganisationId);
            return ObjectMapper.Mapper.Map<List<AddressViewModel>>(addresses);
        }

        public async Task<List<AddressViewModel>> GetAddressBySearchNameAndOrganisationIdAsync(string SearchName, string Email)
        {
            if (string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;

            var addresses = new AddressRepository().GetAddressesBySearchTerm(SearchName, Email);
            return ObjectMapper.Mapper.Map<List<AddressViewModel>>(addresses);
        }

        public async Task<AddressViewModel> GetAddressByIdAsync(Guid Id)
        {
            var result = await new AddressRepository().GetAddressByIdAsync(Id);
            return ObjectMapper.Mapper.Map<AddressViewModel>(result);
        }
    }
}