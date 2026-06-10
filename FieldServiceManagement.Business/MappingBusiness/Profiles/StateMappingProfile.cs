using AutoMapper;
using FieldServiceManagement.Data.DataModels.State;
using FieldServiceManagement.ViewModels.State;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class StateMappingProfile : Profile
    {
        public StateMappingProfile()
        {
            CreateMap<State, StateViewModel>().ReverseMap();
        }
    }
}