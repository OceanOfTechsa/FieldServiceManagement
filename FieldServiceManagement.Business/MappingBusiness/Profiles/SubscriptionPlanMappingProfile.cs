using AutoMapper;
using FieldServiceManagement.Data.DataModels.SubscriptionPlan;
using FieldServiceManagement.ViewModels.SubscriptionPlan;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class SubscriptionPlanMappingProfile : Profile
    {
        public SubscriptionPlanMappingProfile()
        {
            CreateMap<SubscriptionPlan, SubscriptionPlanViewModel>().ReverseMap();
        }
    }
}