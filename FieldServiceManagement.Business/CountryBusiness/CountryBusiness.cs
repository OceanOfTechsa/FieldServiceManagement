using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Country;


namespace FieldServiceManagement.Business.CountryBusiness
{
    public class CountryBusiness
    {
        public List<CountryViewModel> GetCountriesBySearchName(string SearchName)
        {
            if (string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;
            var dbModel = new CountryRepository().GetCountriesBySearchName(SearchName);
            return ObjectMapper.Mapper.Map<List<CountryViewModel>>(dbModel);
        }
    }
}
