using AutoMapper;
using FieldServiceManagement.Data.DataModels.Audits;
using FieldServiceManagement.ViewModels.Audit;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class AuditMappingProfile : Profile
    {
        public AuditMappingProfile()
        {
            CreateMap<AuditLog, AuditLogViewModel>().ReverseMap();
        }
    }
}
