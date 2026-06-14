using AutoMapper;
using FieldServiceManagement.Data.DataModels.OrganisationSubscription;
using FieldServiceManagement.ViewModels.OrganisationSubscription;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class OrganisationSubscriptionMappingProfile : Profile
    {
        public OrganisationSubscriptionMappingProfile()
        {
            CreateMap<OrganisationSubscription, OrganisationSubscriptionViewModel>().ReverseMap();
        }
    }
}
