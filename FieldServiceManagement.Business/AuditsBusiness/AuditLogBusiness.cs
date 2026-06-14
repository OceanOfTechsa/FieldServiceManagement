using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.AuditsBusiness
{
    public class AuditLogBusiness
    {
        public async Task<List<AuditLogViewModel>> GetByEntityAsync(string EntityName, AppUserViewModel user)
        {
            var result = await new Repository.Repositories.AuditLogRepository().GetByEntityAsync(EntityName, user.Id.ToString(), user.OrganisationId);
            return ObjectMapper.Mapper.Map<List<AuditLogViewModel>>(result);
        }
    }
}
