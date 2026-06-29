using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Organisation;

namespace FieldServiceManagement.Business
{
    public class OrganisationBusiness
    {
        public async Task<OrganisationViewModel> GetOrganisationById(Guid OrgId)
        {
            var repo = new OrganisationRepository();
            var Org = repo.GetById(OrgId);
            return ObjectMapper.Mapper.Map<OrganisationViewModel>(Org);
        }

        public async Task<OrganisationDetailsViewModel> GetOrganisationDeatailsByIdAsync(Guid OrgId)
        {
            var result = await new OrganisationRepository().GetOrganisationDetailsByIdAsync(OrgId);
            return ObjectMapper.Mapper.Map<OrganisationDetailsViewModel>(result);
        }
    }
}
