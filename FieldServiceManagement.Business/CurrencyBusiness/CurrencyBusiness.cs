using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Currency;

namespace FieldServiceManagement.Business.CurrencyBusiness
{
    public class CurrencyBusiness
    {
        public List<CurrencyViewModel> GetCurrencyBySearchName(string SearchName)
        {
            if(string.IsNullOrEmpty(SearchName))
                SearchName = string.Empty;
            var currency = new CurrencyRepository().GetCurrencyBySearchName(SearchName);
            return ObjectMapper.Mapper.Map<List<CurrencyViewModel>>(currency);
        }
    }
}
