using FieldServiceManagement.Business.EmailBusiness;
using FieldServiceManagement.Data.Extensions;
using FieldServiceManagement.ViewModels.Announcement;

namespace FieldServiceManagement.Business.NotificationBusiness.Notifications
{
    public class AnnouncementNotification : EmailNotification
    {
        private readonly CreateAnnouncementViewModel _model;
        private readonly DateTime _announcementDate;

        public AnnouncementNotification(CreateAnnouncementViewModel model, string toEmail, List<string> bccRecipients)
            : base($"Announcement: {model.Title}", toEmail, cc: null, bcc: string.Join(",", bccRecipients), fileName: string.Empty)
        {
            _model = model;
            _announcementDate = model.AnnouncementDate.SaDateTime();
        }

        protected override string MergeEmailTemplate()
        {
            var template = GetTemplate();
            template = template.Replace("#Title#", _subject);
            template = template.Replace("#year#", DateTime.Now.Year.ToString());
            template = template.Replace("#Content#", EmailContent());
            return template;
        }

        protected override void PopulateSendNotification() { }

        private string EmailContent() =>
            "<p>Hello,</p>" +
            "<p>" +
                $"<strong>{_model.Title}</strong>" +
            "</p>" +
            "<p>" +
                _model.Description +
            "</p>" +
            (!string.IsNullOrWhiteSpace(_model.Notes)
                ? $"<p><em>{_model.Notes}</em></p>"
                : string.Empty) +
            "<p>" +
                "Announcement date: " +
                $"<strong>{_announcementDate.ToString("MMM dd, yyyy hh:mm")} {_announcementDate.ToString("tt").ToUpper()}</strong>" +
            "</p>" +
            "<p>" +
                "This is an automated notification from <strong>OOT FSM</strong>." +
            "</p>";
    }
}