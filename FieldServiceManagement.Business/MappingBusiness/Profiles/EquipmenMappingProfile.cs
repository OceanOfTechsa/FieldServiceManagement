using AutoMapper;
using FieldServiceManagement.Data.DataModels.Equipment;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Equipment;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class EquipmenMappingProfile : Profile
    {
        public EquipmenMappingProfile()
        {
            CreateMap<CreateEquipment, CreateEquipmentViewModel>().ReverseMap();
            CreateMap<EquipmentListItem, EquipmentListViewModel>().ReverseMap();
            CreateMap<Equipment, EquipmentViewModel>().ReverseMap();
        }
    }
}
