using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Contact;

namespace FieldServiceManagement.Business.ContactBusiness
{
    public class ContactBusiness
    {
        public async Task<Guid?> CreateContactAsync(CreateContactViewModel model, string email)
        {
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);
            model.OrganisationId = currentUser.OrganisationId;
            model.CreatedBy = currentUser.Id;
            model.UpdatedBy = currentUser.Id;

            var dbModel = ObjectMapper.Mapper.Map<Contact>(model);
            return await new ContactRepository().CreateContactAsync(dbModel, new LocalIPResolver().GetRoutedIPv4());
        }

        public async Task<List<ContactDetailsViewModel?>> GetContactFullDetailsByIdAsync(Guid id)
        {
            var result = await new ContactRepository().GetContactFullDetailsByIdAsync(id);
            return ObjectMapper.Mapper.Map<List<ContactDetailsViewModel?>>(result);
        }

        public async Task<IEnumerable<ContactListItemViewModel>> GetContactsByUserEmailAsync(string Email)
        {
            var results = await new ContactRepository().GetContactsByUserEmailAsync(Email);
            return ObjectMapper.Mapper.Map<List<ContactListItemViewModel>>(results);
        }
    }
}
