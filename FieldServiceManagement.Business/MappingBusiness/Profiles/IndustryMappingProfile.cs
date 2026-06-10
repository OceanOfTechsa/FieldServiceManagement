using AutoMapper;
using FieldServiceManagement.Data.DataModels.Industry;
using FieldServiceManagement.ViewModels.Industry;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class IndustryMappingProfile : Profile
    {
        public IndustryMappingProfile()
        {
            CreateMap<Industry, IndustryViewModel>().ReverseMap();
            CreateMap<IndustryCategory, IndustryCategoryViewModel>().ReverseMap();
        }
    }
}