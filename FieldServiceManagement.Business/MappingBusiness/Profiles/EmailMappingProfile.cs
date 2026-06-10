using AutoMapper;
using FieldServiceManagement.Data.DataModels.EmailErrorLog;
using FieldServiceManagement.Data.DataModels.EmailLog;
using FieldServiceManagement.ViewModels.EmailErrorLogViewModel;
using FieldServiceManagement.ViewModels.EmailLogViewModel;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class EmailMappingProfile : Profile
    {
        public EmailMappingProfile()
        {
            CreateMap<EmailErrorLog, EmailErrorLogViewModel>().ReverseMap();
            CreateMap<EmailLog, EmailLogViewModel>().ReverseMap();
        }
    }
}