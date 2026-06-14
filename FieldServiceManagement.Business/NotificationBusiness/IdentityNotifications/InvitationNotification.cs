using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.EmailBusiness;
using FieldServiceManagement.Enum;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.NotificationBusiness.IdentityNotifications
{
    public class InvitationNotification : EmailNotification
    {

        private readonly AppUserProfileViewModel _currentUser;
        private readonly UserInvitationViewModel _model;
        private readonly string _tempPass;

        public InvitationNotification(AppUserProfileViewModel currentUser, UserInvitationViewModel model, string tempPass) : base($"Invite: You are invited to join {currentUser?.Organisation?.Name} on OOT FSM", model.Email)
        {
            _currentUser = currentUser!;
            _model = model;
            _tempPass = tempPass;
        }

        protected override string MergeEmailTemplate()
        {
            var template = GetTemplate();

            template = template.Replace("#Title#", _subject);
            template = template.Replace("#year#", DateTime.Now.Year.ToString());
            template = template.Replace("#Content#", EmailContent());

            return template;
        }

        protected override void PopulateSendNotification() {}

        private string EmailContent()
        {
            return
                $"<p><strong>Hello {_model.Name} {_model.Surname},</strong></p>" +

                $"<p>" +
                $"{_currentUser.User.Name} {_currentUser.User.Surname} has invited you to join " +
                $"<strong>{_currentUser.Organisation?.Name}</strong> on OOT FSM." +
                $"</p>" +

                $"<p>" +
                $"Your assigned role is: " +
                $"<strong>{((UserRole)_model?.UserType!).GetDisplayName()}</strong>." +
                $"</p>" +

                $"<p>" +
                $"Your temporary password is: " +
                $"<strong>{_tempPass}</strong>" +
                $"</p>" +

                "<p>Please click the link below to set your password and continue with your account setup:</p>" +

                $"<p>" +
                $"<a href='{AppSettings.BaseUrl}'  style=\"color: #2e7d32; text-decoration: none;\">Set Password & Complete Registration</a>" +
                $"</p>" +

                $"<p>" +
                $"Please note that this invitation will expire on " +
                $"<strong>{_model.ExpiresAt.ToString("MMM dd, yyyy hh:mm")} {_model.ExpiresAt.ToString("tt").ToUpper()}</strong>. " +
                $"We recommend completing your registration before the expiry date." +
                $"</p>" +

                "<p>If you were not expecting this invitation, you may safely ignore this email.</p>";
        }
    }
}