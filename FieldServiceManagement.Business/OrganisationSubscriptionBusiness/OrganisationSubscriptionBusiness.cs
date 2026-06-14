using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.OrganisationSubscription;

namespace FieldServiceManagement.Business
{
    public class OrganisationSubscriptionBusiness
    {
        public async Task<OrganisationSubscriptionViewModel> GetOrganisationSubscriptionByOrgIdAsync(Guid OrgId)
        {
            var result =  await new OrganisationSubscriptionRepository().GetOrganisationSubscriptionByOrgIdAsync(OrgId);
            return ObjectMapper.Mapper.Map<OrganisationSubscriptionViewModel>(result);
        }
    }
}
