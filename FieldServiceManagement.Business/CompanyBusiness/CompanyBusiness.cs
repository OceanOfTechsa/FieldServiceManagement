using FieldServiceManagement.Business.AuditsBusiness;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Data.DataModels.Entity;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.Company;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.CompanyBusiness
{
    public class CompanyBusiness
    {
        public async Task<Guid?> CreateCompanyAsync(CreateCompanyViewModel model, string email)
        {
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);
            model.OrganisationId = currentUser.OrganisationId;
            model.CreatedBy = currentUser.Id;
            model.UpdatedBy = currentUser.Id;

            var company = ObjectMapper.Mapper.Map<Company>(model);
            return await new CompanyRepository().CreateCompanyAsync(company, new LocalIPResolver().GetRoutedIPv4());
        }

        public async Task<CompanyDetailsViewModel> GetCompanyFullDetailsByIdAsync(Guid Id, string email)
        {
            var company = new CompanyRepository().GetById(Id);
            return await BuildCompanyDetailsViewModel(company!, email);
        }

        public async Task<IEnumerable<CompanyListItemViewModel>> GetCompaniesByUserEmailAsync(string Email)
        {
            var results = await new CompanyRepository().GetCompaniesByUserEmailAsync(Email);
            return ObjectMapper.Mapper.Map<List<CompanyListItemViewModel>>(results);
        }

        public async Task<CompanyViewModel?> GetCompanyByIdAsync(Guid Id)
        {
            var result = new CompanyRepository().GetById(Id);
            return ObjectMapper.Mapper.Map<CompanyViewModel?>(result);
        }

        public async Task<Guid?> UpdateCompanyAsync(CompanyViewModel model, string currentUserEmail)
        {
            var dbModel = ObjectMapper.Mapper.Map<Company>(model);
            dbModel.Id = model.Id;
            return await new CompanyRepository().UpdateCompanyAsync(dbModel, currentUserEmail, new LocalIPResolver().GetRoutedIPv4()!);
        }

        public async Task<BusinessResult> RemoveCompanyAddressLink(string CompanyId, Guid AddressId)
        {
            Guid compId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(CompanyId));
            await new EntityAddressRepository().RemoveCompanyAddressLink(compId, AddressId);
            return new BusinessResult { Success = true };
        }

        public async Task<BusinessResult> DeleteCompany(string CompanyId, string userEmail, string? ipAddress = null)
        {
            Guid compId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(CompanyId));

            var result = await new CompanyRepository().DeleteCompanyAsync(compId, userEmail, new LocalIPResolver().GetRoutedIPv4());

            if (!result.Success)
                return new BusinessResult { Success = false, Message = result.ErrorMessage };

            return new BusinessResult { Success = true };
        }

        #region PRIVATE METHODS
        private static async Task<CompanyDetailsViewModel> BuildCompanyDetailsViewModel(Company company, string currentUserEmail)
        {
            var model = new CompanyDetailsViewModel();
            if (company is null)
                return model;

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);
            if (currentUser is null || company.OrganisationId != currentUser.OrganisationId)
                return model;

            model.Company = ObjectMapper.Mapper.Map<CompanyViewModel>(company);

            var orgId = currentUser.OrganisationId;

            var auditLogsTask = new AuditLogBusiness().GetByEntityAsync("Company", company.Id, orgId);
            var createdByTask = new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(company.CreatedBy);
            var linkedAddressesTask = new AddressBusiness.AddressBusiness().GetEntityLinkedAddressesAsync(EntityTypes.Company, company.Id, orgId);
            var contactsTask = new ContactBusiness.ContactBusiness().GetContactsByCompanyId(company.Id, orgId);

            await Task.WhenAll(auditLogsTask, createdByTask, linkedAddressesTask, contactsTask);

            model.AuditLogs = auditLogsTask.Result;
            model.LinkedAddresses = linkedAddressesTask.Result;
            model.Contacts = contactsTask.Result;

            var createdBy = createdByTask.Result;
            model.CreatedBy = createdBy;

            if (company.UpdatedBy.HasValue)
            {
                model.UpdatedBy = company.UpdatedBy.Value == createdBy.Id
                    ? createdBy
                    : await new UserBusiness.UserBusiness().GetUserDetailsByIdAsync(company.UpdatedBy.Value);
            }

            return model;
        }

        #endregion
    }
}