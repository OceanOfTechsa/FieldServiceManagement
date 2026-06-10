using AutoMapper;
using FieldServiceManagement.Data.DataModels.Currency;
using FieldServiceManagement.ViewModels.Currency;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class CurrencyMappingProfile : Profile
    {
        public CurrencyMappingProfile()
        {
            CreateMap<Currency, CurrencyViewModel>().ReverseMap();
        }
    }
}