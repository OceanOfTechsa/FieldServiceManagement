using AutoMapper;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.ViewModels.Address;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<Address, AddressViewModel>().ReverseMap();
        }
    }
}