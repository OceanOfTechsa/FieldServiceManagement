using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Announcement;

namespace FieldServiceManagement.Business.AnnouncementBusiness
{
    public class AnnouncementDeliveryTypeBusiness
    {
        public List<AnnouncementDeliveryTypeViewModel> GetAllAnnouncementDeliveryTypes()
        {
            var dbModel = new AnnouncementDeliveryTypeRepository().GetAllAnnouncementDeliveryTypes();
            return ObjectMapper.Mapper.Map<List<AnnouncementDeliveryTypeViewModel>>(dbModel);
        }
    }
}
