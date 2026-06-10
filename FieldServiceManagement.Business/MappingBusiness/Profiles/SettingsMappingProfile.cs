using AutoMapper;
using FieldServiceManagement.Data.DataModels.Settings;
using FieldServiceManagement.ViewModels.Settings;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class SettingsMappingProfile : Profile
    {
        public SettingsMappingProfile()
        {
            CreateMap<Settings, SettingsViewModel>().ReverseMap();
        }
    }
}