using AutoMapper;
using FieldServiceManagement.Data.DataModels.Contact;
using FieldServiceManagement.ViewModels.Contact;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class ContactMappingProfile : Profile
    {
        public ContactMappingProfile()
        {
            CreateMap<Contact, CreateContactViewModel>().ReverseMap();
            CreateMap<Contact, ContactViewModel>().ReverseMap();
            CreateMap<ContactListItem, ContactListItemViewModel>().ReverseMap();
            CreateMap<ContactDetails, ContactDetailsViewModel>().ReverseMap();
        }
    }
}
