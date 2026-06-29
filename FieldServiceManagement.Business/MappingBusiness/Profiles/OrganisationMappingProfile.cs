using AutoMapper;
using FieldServiceManagement.Data.DataModels.Organisation;
using FieldServiceManagement.ViewModels.Organisation;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class OrganisationMappingProfile : Profile
    {
        public OrganisationMappingProfile()
        {
            CreateMap<Organisation, OrganisationViewModel>().ReverseMap();
            CreateMap<OrganisationDetails, OrganisationDetailsViewModel>().ReverseMap();
        }
    }
}