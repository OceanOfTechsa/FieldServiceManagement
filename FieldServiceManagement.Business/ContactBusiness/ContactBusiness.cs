using FieldServiceManagement.Enum;
using FieldServiceManagement.ViewModels.Contact;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.Business.AuditsBusiness;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Data.DataModels.Shared;

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

        public async Task<ContactDetailsViewModel?> GetContactFullDetailsByIdAsync(Guid id, string email)
        {
            var contact = new ContactRepository().GetById(id);
            return await BuildContactDetailsViewModel(contact!, email);
        }

        public async Task<IEnumerable<ContactListItemViewModel>> GetContactsByUserEmailAsync(string email)
        {
            var results = await new ContactRepository().GetContactsByUserEmailAsync(email);
            return ObjectMapper.Mapper.Map<List<ContactListItemViewModel>>(results);
        }

        public async Task<List<ContactViewModel>> GetContactsByCompanyId(Guid companyId, Guid organisationId)
        {
            var dataModel = await new ContactRepository().GetContactsByCompanyId(companyId, organisationId);
            return ObjectMapper.Mapper.Map<List<ContactViewModel>>(dataModel);
        }

        public async Task<ContactViewModel> GetContactByIdAsync(Guid id)
        {
            var results = new ContactRepository().GetById(id);
            return ObjectMapper.Mapper.Map<ContactViewModel>(results);
        }

        public async Task<BusinessResult> DeleteContact(string contactId, string userEmail, string? ipAddress = null)
        {
            Guid contactGuid;
            try
            {
                contactGuid = Guid.Parse(UrlEncryptionBusiness.DecryptParam(contactId));
            }
            catch
            { 
                return new BusinessResult { Success = false, Message = "Invalid reference." };
            }

            var result = await new ContactRepository().DeleteContactAsync(contactGuid, userEmail, ipAddress ?? new LocalIPResolver().GetRoutedIPv4());

            if (!result.Success)
            { 
                return new BusinessResult { Success = false, Message = result.ErrorMessage };
            }
            
            return new BusinessResult { Success = true };
        }
        
        public async Task<BusinessResult> UpdateContactAsync(ContactViewModel model, string currentUserEmail)
        {
            var dbModel = ObjectMapper.Mapper.Map<Contact>(model);
            dbModel.Id = model.Id;
 
            var result = await new ContactRepository()
                .UpdateContactAsync(dbModel, currentUserEmail, new LocalIPResolver().GetRoutedIPv4());
 
            if (!result.Success)
            {
                 return new BusinessResult { Success = false, Message = result.ErrorMessage };
            }
 
            return new BusinessResult { Success = true };
        }
        
        #region PRIVATE METHODS
        private static async Task<ContactDetailsViewModel> BuildContactDetailsViewModel(Contact contact, string currentUserEmail)
        {
            var model = new ContactDetailsViewModel();
            if (contact is null)
                return model;

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);
            if (currentUser is null || contact.OrganisationId != currentUser.OrganisationId)
                return model;

            model.Contact = ObjectMapper.Mapper.Map<ContactViewModel>(contact);

            var orgId = currentUser.OrganisationId;

            var auditLogsTask = new AuditLogBusiness().GetByEntityAsync("Contact", contact.Id, orgId);
            var createdByTask = new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(contact.CreatedBy);
            var linkedAddressesTask = new AddressBusiness.AddressBusiness().GetEntityLinkedAddressesAsync(EntityTypes.Contact, contact.Id, orgId);
            var organisationTask = new OrganisationBusiness().GetOrganisationById(orgId);
            var companyTask = new CompanyBusiness.CompanyBusiness().GetCompanyByIdAsync(contact.CompanyId);

            await Task.WhenAll(auditLogsTask, createdByTask, linkedAddressesTask, organisationTask, companyTask);

            model.AuditLogs = auditLogsTask.Result;
            model.LinkedAddresses = linkedAddressesTask.Result;
            model.Organisaion = organisationTask.Result;
            model.Company = companyTask.Result ?? new ();

            var createdBy = createdByTask.Result;
            model.CreatedBy = createdBy;

            if (contact.UpdatedBy.HasValue)
            {
                model.UpdatedBy = contact.UpdatedBy.Value == createdBy.Id
                    ? createdBy
                    : await new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(contact.UpdatedBy.Value);
            }

            return model;
        }
        #endregion
    }
}
