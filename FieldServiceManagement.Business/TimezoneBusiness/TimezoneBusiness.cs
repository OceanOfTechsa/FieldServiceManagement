using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Timezone;

namespace FieldServiceManagement.Business.TimezoneBusiness
{
    public class TimezoneBusiness
    {
        public List<TimezoneViewModel> GetTimezoneBySearchName(string SearchName)
        {
            if (string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;
            var timezone = new TimezoneRepository().GetTimezoneBySearchName(SearchName);
            return ObjectMapper.Mapper.Map<List<TimezoneViewModel>>(timezone);
        }
    }
}
