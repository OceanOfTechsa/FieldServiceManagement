using AutoMapper;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Data.DataModels.UserProfileAudit;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<AppUser, AppUserViewModel>().ReverseMap();
            CreateMap<AppUserProfile, AppUserProfileViewModel>().ReverseMap();
            CreateMap<UserProfileAudit, UserProfileAuditViewModel>().ReverseMap();
            CreateMap<UserInvitation, UserInvitationViewModel>().ReverseMap();
            CreateMap<UserProfileDetails, UserProfileDetailsViewModel>().ReverseMap();
        }
    }
}