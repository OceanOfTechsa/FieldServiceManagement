using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.NotificationBusiness.IdentityNotifications;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.UserBusiness
{
    public class UserInvitationBusiness
    {

        public UserInvitationViewModel CheckIfUserInvitationExistsByEmail(string Email, Guid OrganisationId)
        {
            var repo = new UserInvitationRepository();
            return ObjectMapper.Mapper.Map<UserInvitationViewModel>(repo.Find(x =>
                    x.Email == Email &&
                    x.OrganisationId == OrganisationId &&
                    x.ExpiresAt > DateTime.UtcNow));
        }

        public async Task<UserInvitationViewModel> GetUserInvitationByEmail(string Email)
        {
            var invitation = new UserInvitationRepository().GetInvitationByEmail(Email);
            return ObjectMapper.Mapper.Map<UserInvitationViewModel>(invitation);
        }

        public async void CreateUserInvitationAsync(UserInvitationViewModel InvitationModel, AppUserProfileViewModel CurrentUser)
        {
            var model = new UserInvitation{
                Name = InvitationModel.Name,
                Surname = InvitationModel.Surname,
                Email = InvitationModel.Email,
                UserType = InvitationModel.UserType,
                OrganisationId = CurrentUser.User.OrganisationId,
                CreatedBy = CurrentUser.User.Id,
            };
            new UserInvitationRepository().CreateUserInvitationAsync(model);
        }

        public async Task<BusinessResult> SendInvitationReminder(string Email)
        {
            var invitation = await GetUserInvitationByEmail(Email);
            if(invitation == null)
                return BusinessResult.Fail("User invitation not found");

            var organisation = await new OrganisationBusiness().GetOrganisationById(invitation.OrganisationId);
            new InvitationReminderNotification(invitation, organisation.Name).SendNotificationWithoutQueue();
            return BusinessResult.Ok();
        }
    }
}
