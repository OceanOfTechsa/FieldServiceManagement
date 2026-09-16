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

        public async Task<BusinessResult> LinkCompanyAddressesAsync(LinkAddressRequest request, string currentUserEmail)
        {
            if (string.IsNullOrWhiteSpace(request.ServiceAddressId) && string.IsNullOrWhiteSpace(request.BillingAddressId))
                return new BusinessResult { Success = false, Message = "Select at least one address type." };

            Guid companyId;
            Guid? serviceAddressId;
            Guid? billingAddressId;

            try
            {
                companyId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(request.CompanyId));
                serviceAddressId = ParseOptional(request.ServiceAddressId);
                billingAddressId = ParseOptional(request.BillingAddressId);
            }
            catch
            {
                return new BusinessResult { Success = false, Message = "Invalid reference." };
            }

            var results = await new CompanyRepository().LinkCompanyAddressesAsync(
                companyId, serviceAddressId, billingAddressId, currentUserEmail);

            if (!results.Success)
                return new BusinessResult { Success = false, Message = results.ErrorMessage };

            return new BusinessResult { Success = true };
        }

        public async Task<BusinessResult> UnlinkCompanyAddressAsync(Guid entityAddressId, string currentUserEmail)
        {
            var addressRepository = new EntityAddressRepository();

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);
            var entityAddress = await addressRepository.GetEntityAddressByIdAsync(entityAddressId, currentUser.OrganisationId);
            if (entityAddress is null)
                return new BusinessResult { Success = false, Message = "Address link not found." };

            var unlinked = await addressRepository.UnlinkEntityAddressAsync(entityAddress, entityAddressId, currentUser.OrganisationId, currentUser.Id);
            if (!unlinked)
                return new BusinessResult { Success = false, Message = "Couldn't unlink the address." };

            LogFieldChangeAsync(entityAddress, currentUser);

            return new BusinessResult { Success = true };
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

        private static Guid? ParseOptional(string? encrypted)
        {
            return string.IsNullOrWhiteSpace(encrypted) ? null : Guid.Parse(UrlEncryptionBusiness.DecryptParam(encrypted));
        }

        private static string NormaliseAddressTypeName(int EntityAddressId)
        {
            return (AddressTypes)EntityAddressId switch
            {
                AddressTypes.Service => "Service",
                AddressTypes.Billing => "Billing",
                AddressTypes.Home => "Home",
                AddressTypes.Work => "Work",
                AddressTypes.Postal => "Postal",
                AddressTypes.Delivery => "Delivery",
                _ => "Address"
            };
        }

        private static async void LogFieldChangeAsync(EntityAddress entityAddress, AppUserViewModel currentUser)
        {
            var addressTypeName = NormaliseAddressTypeName(entityAddress.AddressTypeId);

            await new AuditLogBusiness().LogFieldChangeAsync(new AuditLogViewModel
            {
                EntityName = "Company",
                EntityId = entityAddress.EntityId.ToString(),
                Action = "Updated",
                FieldName = $"{addressTypeName} Address",
                OldValue = entityAddress.AddressId.ToString(),
                NewValue = null,
                PerformedByUserId = currentUser.Id,
                PerformedByName = $"{currentUser.Name} {currentUser.Surname}",
                Comment = $"{currentUser.Name} {currentUser.Surname} Unliked {addressTypeName} address.",
                OrganisationId = currentUser.OrganisationId,
                IpAddress = null,
                ShowComment = true
            });
        }

        #endregion
    }
}