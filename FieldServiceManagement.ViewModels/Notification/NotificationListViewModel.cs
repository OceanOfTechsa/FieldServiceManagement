namespace FieldServiceManagement.ViewModels.Notification
{
    public class NotificationListViewModel
    {
        public List<UserNotificationViewModel> Notifications { get; set; } = new();
        public int UnreadCount { get; set; }
    }
}
