using AutoMapper;
using FieldServiceManagement.Data.DataModels.Language;
using FieldServiceManagement.ViewModels.Language;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class LanguageMappingProfile : Profile
    {
        public LanguageMappingProfile()
        {
            CreateMap<Language, LanguageViewModel>().ReverseMap();
        }
    }
}