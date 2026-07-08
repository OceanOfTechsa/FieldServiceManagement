using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data.DataModels.Notification;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Notification;

namespace FieldServiceManagement.Business.NotificationBusiness
{
    public class UserNotificationBusiness
    {
        public async Task<NotificationListViewModel> GetNotificationsForUserAsync(string email, NotificationFilterViewModel filter)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new NotificationListViewModel();

            var repo = new NotificationRepository();

            var dataFilter = ObjectMapper.Mapper.Map<NotificationFilter>(filter);
            var dbModel = await repo.GetNotificationsForUserAsync(email, dataFilter);
            var notifications = ObjectMapper.Mapper.Map<List<UserNotificationViewModel>>(dbModel);

            var unreadCount = notifications.Count(n => !n.IsRead);

            var viewModel = new NotificationListViewModel
            {
                Notifications = notifications,
                UnreadCount = unreadCount
            };

            return viewModel;
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid Id, string email)
        {
            if (string.IsNullOrWhiteSpace(email) || Id == Guid.Empty)
                return false;

            return await new NotificationRepository().MarkNotificationAsReadAsync(Id, email);
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await new NotificationRepository().MarkAllNotificationsAsReadAsync(email);
        }

        public async Task<int> GetUnreadNotificationCountAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return 0;

            return await new NotificationRepository().GetUnreadNotificationCountAsync(email);
        }
    }
}