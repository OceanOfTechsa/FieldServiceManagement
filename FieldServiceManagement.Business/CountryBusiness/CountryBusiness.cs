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

        public async Task<List<CountryViewModel>> GetAllCountries()
        {
            var dbModel = await new CountryRepository().GetAllCountries();
            return ObjectMapper.Mapper.Map<List<CountryViewModel>>(dbModel);
        }
    }
}
