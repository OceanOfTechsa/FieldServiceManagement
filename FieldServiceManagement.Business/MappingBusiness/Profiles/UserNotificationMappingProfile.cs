using AutoMapper;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Notification;
using FieldServiceManagement.ViewModels.Address;
using FieldServiceManagement.ViewModels.Notification;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class UserNotificationMappingProfile : Profile
    {
        public UserNotificationMappingProfile()
        {
            CreateMap<UserNotification, UserNotificationViewModel>().ReverseMap();
            CreateMap<NotificationFilter, NotificationFilterViewModel>().ReverseMap();
        }
    }
}
