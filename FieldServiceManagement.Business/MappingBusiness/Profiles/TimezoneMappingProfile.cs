using AutoMapper;
using FieldServiceManagement.Data.DataModels.Timezone;
using FieldServiceManagement.ViewModels.Timezone;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class TimezoneMappingProfile : Profile
    {
        public TimezoneMappingProfile()
        {
            CreateMap<Timezone, TimezoneViewModel>().ReverseMap();
        }
    }
}