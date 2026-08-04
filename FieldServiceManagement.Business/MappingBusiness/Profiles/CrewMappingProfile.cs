using AutoMapper;
using FieldServiceManagement.ViewModels.Crew;
using FieldServiceManagement.Data.DataModels.Crew;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class CrewMappingProfile : Profile
    {
        public CrewMappingProfile() 
        {
            CreateMap<Crew, CrewViewModel>().ReverseMap();
            CreateMap<CrewListItem, CrewListItemViewModel>().ReverseMap();
            CreateMap<CreateCrew, CreateCrewViewModel>().ReverseMap();
            CreateMap<CrewDetails, CrewDetailsViewModel>().ReverseMap();
            CreateMap<CrewSearchResult, CrewSearchResultViewModel>().ReverseMap();
        }
    }
}
