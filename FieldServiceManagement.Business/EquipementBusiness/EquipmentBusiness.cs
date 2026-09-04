using FieldServiceManagement.ViewModels.Equipment;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Equipment;
using FieldServiceManagement.Models;
using FieldServiceManagement.Business.AuditsBusiness;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.User;
using FieldServiceManagement.Data.DataModels.Shared;

namespace FieldServiceManagement.Business.EquipementBusiness
{
    public class EquipmentBusiness
    {

        public async Task<EquipmentViewModel> GetEquipmentByIdAsync(Guid Id)
        {
            var results = new EquipmentRepository().GetById(Id);
            return ObjectMapper.Mapper.Map<EquipmentViewModel>(results);
        }
        public async Task<IEnumerable<EquipmentListViewModel>> GetEquipmentsByOrganisationId(Guid OrgId)
        {
            var results = await new EquipmentRepository().GetEquipmentsByOrganisationIdAsync(OrgId);
            return ObjectMapper.Mapper.Map<List<EquipmentListViewModel>>(results);
        }

        public async Task<Guid?> CreateEquipment(CreateEquipmentViewModel Model)
        {
            var dbModel = ObjectMapper.Mapper.Map<CreateEquipment>(Model);
            return await new EquipmentRepository().CreateEquipmentAsync(dbModel);
        }

        public async Task<EquipmentDetailsViewModel?> GetEquipmentDetailsAsync(Guid Id, string currentUserEmail)
        {
            var equipment = new EquipmentRepository().GetById(Id);
            return await BuildEquipmentDetails(equipment!, currentUserEmail);
        }

        public async Task<BusinessResult> UpdateEquipmentAsync(EquipmentViewModel model, string email)
        {
            var dataModel = ObjectMapper.Mapper.Map<Equipment>(model);
            var result = await new EquipmentRepository().UpdateEquipmentAsync(dataModel, email);
            if (!result.Success)
                return BusinessResult.Fail("Failed to update Equipment");
            return BusinessResult.Ok();
        }



        #region PRIVATE METHODS
        private async Task<EquipmentDetailsViewModel> BuildEquipmentDetails(Equipment equipment, string currentUserEmail)
        {
            var model = new EquipmentDetailsViewModel();
            if (equipment is null)
                return model;

            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(currentUserEmail);
            if(equipment.OrganisationId != currentUser.OrganisationId)
                return model;

            model.Equipment = ObjectMapper.Mapper.Map<EquipmentViewModel>(equipment);
            model.AuditLogs = ObjectMapper.Mapper.Map<List<AuditLogViewModel>>(await new AuditLogBusiness().GetByEntityAsync("Equipment", equipment.Id, currentUser.OrganisationId));
            model.CreatedBy = ObjectMapper.Mapper.Map<AppUserProfileViewModel>(await new UserBusiness.UserBusiness().GetAllUserDetailsByIdAsync(equipment.CreatedBy));
            model.EquipmentOwner = ObjectMapper.Mapper.Map<AppUserProfileViewModel>(await new UserBusiness.UserBusiness().GetAllUserDetailsByIdAsync(equipment.OwnerId));
            if(equipment.UpdatedBy.HasValue)
                model.UpdatedBy = ObjectMapper.Mapper.Map<AppUserProfileViewModel>(await new UserBusiness.UserBusiness().GetAllUserDetailsByIdAsync(equipment.UpdatedBy.Value));

            return model;
        }
        #endregion
    }
}
