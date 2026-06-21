using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.State;

namespace FieldServiceManagement.Business.StateBusiness
{
    public class StateBusiness
    {
        public List<StateViewModel> GetStateBySearchName(string SearchName, int CountryId)
        {
            if(string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;
            var dbModel = new StateRepository().GetStateBySearchName(SearchName, CountryId);
            return ObjectMapper.Mapper.Map<List<StateViewModel>>(dbModel);  
        }

        public async Task<List<StateViewModel>> GetAllStates()
        {
            var dbModel = await new StateRepository().GetAllStates();
            return ObjectMapper.Mapper.Map<List<StateViewModel>>(dbModel);
        }
    }
}
