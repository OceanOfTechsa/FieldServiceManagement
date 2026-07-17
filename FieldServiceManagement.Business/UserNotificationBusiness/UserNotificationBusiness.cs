using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Notification;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Announcement;
using FieldServiceManagement.ViewModels.Notification;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Business.NotificationBusiness
{
    public class UserNotificationBusiness
    {
        public async Task<NotificationListViewModel> GetNotificationsForUserAsync(string email, NotificationFilterViewModel filter)
        {
            if (!IsEmailValid(email))
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
            if (!IsEmailValid(email) || Id == Guid.Empty)
                return false;
            return await new NotificationRepository().MarkNotificationAsReadAsync(Id, email);
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(string email)
        {
            if (!IsEmailValid(email))
                return false;
            return await new NotificationRepository().MarkAllNotificationsAsReadAsync(email);
        }

        public async Task<int> GetUnreadNotificationCountAsync(string email)
        {
            if (!IsEmailValid(email))
                return 0;
            return await new NotificationRepository().GetUnreadNotificationCountAsync(email);
        }

        //PREVENT DUPLICATES
        public async Task<bool> CreateUserAnnouncementNotificationAsync(CreateAnnouncementViewModel model)
        {
            if (model.SelectedRoleIds == null || !model.SelectedRoleIds.Any())
                return false;

            var notification = new CreateUserNotification
            {
                RoleIds = model.SelectedRoleIds,
                Type = "announcement",
                Title = model.Title,
                Message = model.Description,
                RelatedEntityType = "Announcement",
                Severity = (byte)(model.Severity ?? 0),
                CreatedByEmail = model.CreatedByEmail!,
            };

            return await new NotificationRepository().CreateNotificationsBulkAsync(notification);
        }


        #region  PRIVATE METHODS
        private static bool IsEmailValid(string? email)
        {
            return !string.IsNullOrWhiteSpace(email)
                   && new EmailAddressAttribute().IsValid(email);
        }
        #endregion
    }
}