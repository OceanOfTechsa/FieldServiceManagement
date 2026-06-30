using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.EmailBusiness;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.NotificationBusiness.IdentityNotifications
{
    public class InvitationReminderNotification : EmailNotification
    {
        private readonly UserInvitationViewModel _model;
        private readonly string _organisationName;

        public InvitationReminderNotification(
            UserInvitationViewModel model,
            string organisationName)
            : base(
                $"Reminder: Complete your invitation to join {organisationName} on OOT FSM",
                model.Email)
        {
            _model = model;
            _organisationName = organisationName;
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

        private string EmailContent()
        {
            return
                $"<p> <strong>Hello {_model.Name} {_model.Surname},</strong></p>" +

                $"<p>" +
                $"This is a friendly reminder that you have been invited to join " +
                $"<strong>{_organisationName}</strong> on OOT FSM." +
                $"</p>" +

                "<p>" +
                $"Please use the link below to accept your invitation and complete your account setup." +
                "</p>" +

                $"<p>" +
                $"<a href='{AppSettings.baseUrl}'  style=\"color: #2e7d32; text-decoration: none;\">Accept Invitation</a>" +
                $"</p>" +

                $"<p>" +
                $"Please note that your invitation will expire on " +
                $"<strong>" +
                $"{_model.ExpiresAt.ToString("MMM dd, yyyy hh:mm")} " +
                $"{_model.ExpiresAt.ToString("tt").ToUpper()}" +
                $"</strong>." +
                $"</p>" +

                "<p>" +
                $"If you have already completed your registration, please disregard this email." +
                "</p>";
        }
    }
}