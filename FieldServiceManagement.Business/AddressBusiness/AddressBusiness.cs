using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Repository;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Contact;

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

        public async Task<IEnumerable<AddressListItemViewModel>> GetAddressByUserEmailAsync(string Email)
        {
            var results = await new AddressRepository().GetAddressesByUserEmailAsync(Email);
            return ObjectMapper.Mapper.Map<List<AddressListItemViewModel>>(results);
        }

        public async Task<BusinessResult> CreateAddress(CreateAddressViewModel Model, string currentUserEmail)
        {
            var user = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);

            Model.CreatedById = user.Id;
            Model.UpdatedById = user.Id;
            Model.OrganisationId = user.OrganisationId;
            Model.IpAddress = new LocalIPResolver().GetRoutedIPv4();

            var dbModel = ObjectMapper.Mapper.Map<CreateAddress>(Model);
            var (id, success) = await new AddressRepository().CreateAddress(dbModel);

            if (!success)
                return BusinessResult.Fail("Address could not be saved. Please try again.");

            return BusinessResult.Ok();
        }
    }
}