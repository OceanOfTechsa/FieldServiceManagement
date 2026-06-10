using FieldServiceManagement.Business.EmailBusiness;

namespace FieldServiceManagement.Business.NotificationBusiness.IdentityNotifications
{
    public class WelcomeNotification : EmailNotification
    {
        private readonly string _confirmationLink;

        public WelcomeNotification(string toEmail, string confirmationLink) : base("Signup: Welcome to OOT FSM! We are excited to have you on board", toEmail)
        {
            _confirmationLink = confirmationLink;
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
                "Welcome to <strong>OOT FSM</strong>. " +
                "Thank you for creating an account with us." +
            "</p>" +
            "<p>" +
                "To activate your account and verify your email address, please click the link below:" +
            "</p>" +
            $"<p>" +
                $"<a href='{_confirmationLink}'>Confirm Email Address</a>" +
            $"</p>" +
            "<p>" +
                "For security purposes, this confirmation link will expire in <strong>24 hours</strong>." +
            "</p>" +
            "<p>" +
                "If you did not create this account, you may safely ignore this email." +
            "</p>";
    }
}
