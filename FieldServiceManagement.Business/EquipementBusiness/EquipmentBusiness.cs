using FieldServiceManagement.ViewModels.Equipment;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Equipment;

namespace FieldServiceManagement.Business.EquipementBusiness
{
    public class EquipmentBusiness
    {
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

        public async Task<List<EquipmentDetailsViewModel?>> GetEquipmentDetailsAsync(Guid Id)
        {
            var results = await new EquipmentRepository().GetEquipmentFullDetailsByIdAsync(Id);
            return ObjectMapper.Mapper.Map<List<EquipmentDetailsViewModel?>>(results);
        }
    }
}
