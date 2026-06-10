using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Language;

namespace FieldServiceManagement.Business.LanguageBusiness
{
    public class LanguageBusiness
    {
        public List<LanguageViewModel> GetLanguagesBySearchName(string SearchName)
        {
            if(string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;
            var Languages = new LanguageRepository().GetLanguageBySearchName(SearchName);   
            return ObjectMapper.Mapper.Map<List<LanguageViewModel>>(Languages);
        }

        public List<LanguageViewModel> GetLanguages()
        {
            var Languages = new LanguageRepository().GetLanguages();
            return ObjectMapper.Mapper.Map<List<LanguageViewModel>>(Languages);
        }
    }
}
