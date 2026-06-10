using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Industry;

namespace FieldServiceManagement.Business.IndustryBusiness
{
    public class IndustryBusiness
    {
        public List<IndustryViewModel> GetIndustryBySearchName(string SearchName)
        {
            if (string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;
            var dbModel = new IndustryRepository().GetIndustryBySearchName(SearchName);
            return ObjectMapper.Mapper.Map<List<IndustryViewModel>>(dbModel);
        }

        public List<IndustryCategoryViewModel> GetIndustryCategoryBySearchNameAndIndustryId(string SearchName, int industryId)
        {
            if (string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;
            var dbModel = new IndustryRepository().GetIndustryCategoryBySearchNameAndIndustryId(SearchName, industryId);
            return ObjectMapper.Mapper.Map<List<IndustryCategoryViewModel>>(dbModel);
        }
    }
}
