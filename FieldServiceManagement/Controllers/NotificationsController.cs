using FieldServiceManagement.Business.NotificationBusiness;
using FieldServiceManagement.Models;
using FieldServiceManagement.ViewModels.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(
            bool? isRead,
            byte? severity,
            string type,
            DateTime? dateFrom,
            DateTime? dateTo,
            string searchTerm)
        {
            var email = User.Identity?.Name;

            var filter = new NotificationFilterViewModel
            {
                IsRead = isRead,
                Severity = severity,
                Type = type,
                DateFrom = dateFrom,
                DateTo = dateTo,
                SearchTerm = searchTerm
            };

            var result = await new UserNotificationBusiness().GetNotificationsForUserAsync(email, filter);

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
            var email = User.Identity?.Name;

            var success = await new UserNotificationBusiness().MarkAllNotificationsAsReadAsync(email);

            return Json(new { success });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var email = User.Identity?.Name;

            var count = await new UserNotificationBusiness().GetUnreadNotificationCountAsync(User.Identity?.Name!);

            return Json(new { count });
        }


        public class MarkNotificationAsReadRequest
        {
            public string Id { get; set; }
        }
    }
}