using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Audits;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.Repository.Repositories;

namespace FieldServiceManagement.Business.AuditsBusiness
{
    public class AuditLogBusiness
    {
        public async Task LogFieldChangeAsync(AuditLogViewModel model)
        {
            var auditLog = ObjectMapper.Mapper.Map<AuditLog>(model);
            await new AuditLogRepository().InsertAsync(auditLog);
        }

        public async Task<List<AuditLogViewModel>> GetByEntityAsync(string EntityName, Guid EntityId, Guid OrgId)
        {
            var result = await new AuditLogRepository().GetByEntityAsync(EntityName, EntityId.ToString(), OrgId);
            return ObjectMapper.Mapper.Map<List<AuditLogViewModel>>(result);
        }
    }
}
