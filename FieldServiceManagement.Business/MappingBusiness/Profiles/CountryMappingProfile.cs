using AutoMapper;
using FieldServiceManagement.Data.DataModels.Country;
using FieldServiceManagement.ViewModels.Country;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class CountryMappingProfile : Profile
    {
        public CountryMappingProfile()
        {
            CreateMap<Country, CountryViewModel>().ReverseMap();
        }
    }
}