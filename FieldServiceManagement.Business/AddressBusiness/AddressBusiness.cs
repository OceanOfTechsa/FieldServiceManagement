using FieldServiceManagement.Business.AuditsBusiness;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Entity;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.AddressBusiness
{
    public class AddressBusiness
    {
        public async Task<List<AddressViewModel>> GetAllAddressesByOrganisationIdAsync(Guid organisationId)
        {
            var addresses = new AddressRepository().GetAllAddressesByOrganisaId(organisationId);
            return ObjectMapper.Mapper.Map<List<AddressViewModel>>(addresses);
        }

        public async Task<List<AddressViewModel>> GetAvailableAddressesForCompanyAsync(Guid organisationId, Guid? currentCompanyId = null)
        {
            var addresses = await new AddressRepository().GetAvailableAddressesForCompanyAsync(organisationId, currentCompanyId);
            return ObjectMapper.Mapper.Map<List<AddressViewModel>>(addresses);
        }

        public async Task<List<EntityLinkedAddressViewModel>> GetEntityLinkedAddressesAsync(EntityTypes entityType, Guid entityId, Guid organisationId)
        {
            var data = await new AddressRepository().GetEntityLinkedAddressesAsync((int)entityType, entityId, organisationId);
            return ObjectMapper.Mapper.Map<List<EntityLinkedAddressViewModel>>(data);
        }

        public async Task<List<AddressViewModel>> GetAddressBySearchNameAndOrganisationIdAsync(string searchName, string email)
        {
            if (string.IsNullOrEmpty(searchName))
                searchName = string.Empty;

            var addresses = new AddressRepository().GetAddressesBySearchTerm(searchName, email);
            return ObjectMapper.Mapper.Map<List<AddressViewModel>>(addresses);
        }

        public async Task<AddressViewModel> GetAddressByIdAsync(Guid addressId)
        {
            var result = await new AddressRepository().GetAddressByIdAsync(addressId);
            return ObjectMapper.Mapper.Map<AddressViewModel>(result);
        }

        public async Task<IEnumerable<AddressListItemViewModel>> GetAddressByUserEmailAsync(string email)
        {
            var results = await new AddressRepository().GetAddressesByUserEmailAsync(email);
            return ObjectMapper.Mapper.Map<List<AddressListItemViewModel>>(results);
        }

        public async Task<BusinessResult> CreateAddress(CreateAddressViewModel model, string currentUserEmail)
        {
            var user = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);

            model.CreatedById = user.Id;
            model.UpdatedById = user.Id;
            model.OrganisationId = user.OrganisationId;
            model.IpAddress = new LocalIPResolver().GetRoutedIPv4();

            var dbModel = ObjectMapper.Mapper.Map<CreateAddress>(model);
            var (id, success) = await new AddressRepository().CreateAddress(dbModel);

            if (!success)
                return BusinessResult.Fail("Address could not be saved. Please try again.");

            return BusinessResult.Ok();
        }

        public async Task<BusinessResult> LinkEntityAddressesAsync(string entityName, LinkAddressRequest request, string currentUserEmail)
        {
            if (string.IsNullOrWhiteSpace(request.ServiceAddressId) && string.IsNullOrWhiteSpace(request.BillingAddressId))
                return new BusinessResult { Success = false, Message = "Select at least one address type." };

            Guid entityId;
            Guid? serviceAddressId;
            Guid? billingAddressId;

            try
            {
                entityId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(request.EntityId));
                serviceAddressId = ParseOptional(request.ServiceAddressId);
                billingAddressId = ParseOptional(request.BillingAddressId);
            }
            catch
            {
                return new BusinessResult { Success = false, Message = "Invalid reference." };
            }

            var results = await new EntityAddressRepository().LinkEntityAddressesAsync(
                entityName, entityId, serviceAddressId, billingAddressId, currentUserEmail);

            if (!results.Success)
                return new BusinessResult { Success = false, Message = results.ErrorMessage };

            return new BusinessResult { Success = true };
        }


        public async Task<BusinessResult> UnlinkEntityAddressAsync(string entityName, Guid entityId, Guid entityAddressId, string currentUserEmail)
        {
            var addressRepository = new EntityAddressRepository();

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);
            var entityAddress = await addressRepository.GetEntityAddressByIdAsync(entityAddressId, currentUser.OrganisationId);

            // The link must exist AND belong to the entity the page was opened for.
            // Without the EntityId check, a user could unlink any link in the organisation
            // by posting a different entityAddressId.
            if (entityAddress is null || entityAddress.EntityId != entityId)
                return new BusinessResult { Success = false, Message = "Address link not found." };

            var unlinked = await addressRepository.UnlinkEntityAddressAsync(entityAddress, currentUser.Id);
            if (!unlinked)
                return new BusinessResult { Success = false, Message = "Couldn't unlink the address." };

            await LogUnlinkAsync(entityName, entityAddress, currentUser);

            return new BusinessResult { Success = true };
        }


        public async Task<BusinessResult> RemoveEntityAddressLinkAsync(string encryptedEntityId, Guid addressId, Guid organisationId)
        {
            Guid entityId;
            try
            {
                entityId = Guid.Parse(UrlEncryptionBusiness.DecryptParam(encryptedEntityId));
            }
            catch
            {
                return new BusinessResult { Success = false, Message = "Invalid reference." };
            }

            var result = await new EntityAddressRepository().RemoveEntityAddressLinkAsync(entityId, addressId, organisationId);

            if (!result.Success)
                return new BusinessResult { Success = false, Message = result.ErrorMessage };

            return new BusinessResult { Success = true };
        }

        #region PRIVATE METHODS
        private static Guid? ParseOptional(string? encrypted)
        {
            return string.IsNullOrWhiteSpace(encrypted) ? null : Guid.Parse(UrlEncryptionBusiness.DecryptParam(encrypted));
        }

        private static string NormaliseAddressTypeName(int entityAddressId)
        {
            return (AddressTypes)entityAddressId switch
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


        private static async Task LogUnlinkAsync(string entityName, EntityAddress entityAddress, AppUserViewModel currentUser)
        {
            var addressTypeName = NormaliseAddressTypeName(entityAddress.AddressTypeId);
            var performedBy = $"{currentUser.Name} {currentUser.Surname}";

            await new AuditLogBusiness().LogFieldChangeAsync(new AuditLogViewModel
            {
                EntityName = entityName,        
                EntityId = entityAddress.EntityId.ToString(),
                Action = "Updated",
                FieldName = $"{addressTypeName} Address",
                OldValue = entityAddress.AddressId.ToString(),
                NewValue = null,
                PerformedByUserId = currentUser.Id,
                PerformedByName = performedBy,
                Comment = $"{performedBy} unlinked {addressTypeName.ToLower()} address.",
                OrganisationId = currentUser.OrganisationId,
                IpAddress = null,
                ShowComment = true
            });
        }

        #endregion
    }
}