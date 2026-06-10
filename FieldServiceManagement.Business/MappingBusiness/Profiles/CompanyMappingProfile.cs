using AutoMapper;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.ViewModels.Company;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class CompanyMappingProfile : Profile
    {
        public CompanyMappingProfile()
        {
            CreateMap<Company, CreateCompanyViewModel>().ReverseMap();
            CreateMap<CompanyResult, CompanyListItemViewModel>().ReverseMap();
            CreateMap<Company, CompanyViewModel>().ReverseMap();
            CreateMap<CompanyDetails, CompanyDetailsViewModel>().ReverseMap();
        }
    }
}