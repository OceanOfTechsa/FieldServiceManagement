using FieldServiceManagement.Business.EmailBusiness;
using FieldServiceManagement.Data.Extensions;

namespace FieldServiceManagement.Business.NotificationBusiness.IdentityNotifications
{
    public class DuplicateRegistrationNotification : EmailNotification
    {
        private readonly DateTime _currentDate;
        private readonly string? _IpAddress;

        public DuplicateRegistrationNotification(string toEmail) : base("Security Notice: Registration Attempt", toEmail)
        {
            _currentDate = DateTime.Now.SaDateTime();
            _IpAddress = new LocalIPResolver().GetRoutedIPv4() ?? string.Empty;
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
                "We received a request to create a new <strong>OOT FSM</strong> account using your email address." +
            "</p>" +
            "<p>" +
                "However, an account with this email address already exists." +
            "</p>" +
            "<p>" +
                "This attempt was made on " +
                $"<strong>{_currentDate.ToString("MMM dd, yyyy hh:mm")} {_currentDate.ToString("tt").ToUpper()}</strong>" +
                (!string.IsNullOrEmpty(_IpAddress) ? $" from IP address <strong>{_IpAddress}</strong>" : string.Empty) +
                "." +
            "</p>" +
            "<p>" +
                "If you have forgotten your password, you can reset it by clicking the link below:" +
            "</p>" +
            "<p>" +
                "<a href='/Identity/Account/ForgotPassword' style=\"color: #2e7d32; text-decoration: none;\">Reset Password</a>" +
            "</p>" +
            "<p>" +
                "If you did not attempt to register, you can safely ignore this email. " +
                "Your account remains secure and no changes have been made." +
            "</p>";
    }
}