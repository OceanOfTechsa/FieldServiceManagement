using AutoMapper;
using FieldServiceManagement.Data.DataModels.Announcement;
using FieldServiceManagement.ViewModels.Announcement;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class AnnouncementMappingProfile : Profile
    {
        public AnnouncementMappingProfile()
        {
            CreateMap<Announcement, AnnouncementViewModel>().ReverseMap();
            CreateMap<UserAnnouncement, UserAnnouncementViewModel>().ReverseMap();
            CreateMap<AnnouncementAdminListItem, AnnouncementAdminListItemViewModel>().ReverseMap();
            CreateMap<AnnouncementDetail, AnnouncementDetailViewModel>().ReverseMap();
            CreateMap<CreateAnnouncementViewModel, CreateAnnouncementModel>()
               .ForMember(dest => dest.VisibleToRoleIds, opt => opt.MapFrom(src =>
                   src.SelectedRoleIds != null && src.SelectedRoleIds.Any()
                       ? string.Join(",", src.SelectedRoleIds)
                       : null));
        }
    }
}
