using FieldServiceManagement.Business.NotificationBusiness;
using FieldServiceManagement.ViewModels.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(bool? isRead, byte? severity, string type, DateTime? dateFrom, DateTime? dateTo, string searchTerm)
        {
            var filter = PopulateNotificationFilterViewModel(isRead, severity, type, dateFrom, dateTo, searchTerm);
            var result = await new UserNotificationBusiness().GetNotificationsForUserAsync(User.Identity?.Name!, filter);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> MarkNotificationAsRead(Guid Id)
        { 
            if (Id == Guid.Empty)
                return Json(new { success = false, message = "Notification id is required." });

            var success = await new UserNotificationBusiness().MarkNotificationAsReadAsync(Id, User.Identity?.Name!);
            return Json(new { success });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var success = await new UserNotificationBusiness().MarkAllNotificationsAsReadAsync(User.Identity?.Name!);
            return Json(new { success });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var count = await new UserNotificationBusiness().GetUnreadNotificationCountAsync(User.Identity?.Name!);
            return Json(new { count });
        }

        #region PRIVATE METHODS
        private NotificationFilterViewModel PopulateNotificationFilterViewModel(bool? isRead, byte? severity, string type, DateTime? dateFrom, DateTime? dateTo, string searchTerm)
        {
            var filter = new NotificationFilterViewModel
            {
                IsRead = isRead,
                Severity = severity,
                Type = type,
                DateFrom = dateFrom,
                DateTo = dateTo,
                SearchTerm = searchTerm
            };
            return filter;
        }
        #endregion
    }
}