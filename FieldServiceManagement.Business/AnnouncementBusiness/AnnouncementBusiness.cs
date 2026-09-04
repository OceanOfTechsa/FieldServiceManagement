using FieldServiceManagement.Business.Configuration;
using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.NotificationBusiness;
using FieldServiceManagement.Business.NotificationBusiness.Notifications;
using FieldServiceManagement.Data.DataModels.Announcement;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Announcement;
using FieldServiceManagement.ViewModels.Extensions;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Business.AnnouncementBusiness
{
    public class AnnouncementBusiness
    {
        public async Task<AnnouncementViewModel> GetAnnouncementById(Guid AnnouncementId)
        {
            var dbModel = await new AnnouncementRepository().GetAnnouncementById(AnnouncementId);
            return ObjectMapper.Mapper.Map<AnnouncementViewModel>(dbModel);
        }

        public async Task<AnnouncementListViewModel> GetAnnouncementsForUserAsync(string Email)
        {
            if (!IsEmailValid(Email))
                return new AnnouncementListViewModel();
            
            var dbModel = await new AnnouncementRepository().GetAnnouncementsForUserAsync(Email);
            var announcements = ObjectMapper.Mapper.Map<List<UserAnnouncementViewModel>>(dbModel);
            var viewModel = new AnnouncementListViewModel
            {
                Announcements = announcements
            };
            return viewModel;
        }

        public async Task<bool> MarkAnnouncementAsSeenAsync(Guid AnnouncementId, string Email)
        {
            if (!IsEmailValid(Email))
                return false;
            await new AnnouncementRepository().MarkAnnouncementAsSeenAsync(AnnouncementId, Email);
            return true;
        }

        public async Task<bool> MarkBulkAnnouncementsAsSeenAsync(List<Guid> AnnouncementIds, string Email)
        {
            if (!AnnouncementIds.Any())
                return false;

            var repo = new AnnouncementRepository();
            foreach (var id in AnnouncementIds)
                await repo.MarkAnnouncementAsSeenAsync(id, Email);
            return true;
        }

        public async Task<int> GetUnseenAnnouncementCountAsync(string Email)
        {
            if (!IsEmailValid(Email))
                return 0;
            return await new AnnouncementRepository().GetUnseenAnnouncementCountAsync(Email);
        }
        public async Task<Guid> CreateAnnouncementAsync(CreateAnnouncementViewModel Model)
        {
            if (Model == null || string.IsNullOrWhiteSpace(Model.Title))
                return Guid.Empty;

            return await ProcessAnnouncementDeliveryAsync(Model);
        }

        public async Task<List<AnnouncementAdminListItemViewModel>> GetAllAnnouncementsForAdminAsync()
        {
            var dbModel = await new AnnouncementRepository().GetAllAnnouncementsForAdminAsync();
            return ObjectMapper.Mapper.Map<List<AnnouncementAdminListItemViewModel>>(dbModel);
        }

        public async Task<AnnouncementDetailViewModel> GetAnnouncementDetailsByIdAsync(Guid AnnouncementId)
        {
            var dbModel = await new AnnouncementRepository().GetAnnouncementDetailByIdAsync(AnnouncementId);
            return ObjectMapper.Mapper.Map<AnnouncementDetailViewModel>(dbModel);
        }
        public async Task<bool> UpdateAnnouncementAsync(EditAnnouncementViewModel Model, string Email)
        {
            var repo = new AnnouncementRepository();
            var existing = await repo.GetAnnouncementById(Model.Id);

            if (existing == null)
                return false;

            var visibleToRoleIds = Model.SelectedRoleIds != null && Model.SelectedRoleIds.Any()
                ? string.Join(",", Model.SelectedRoleIds)
                : null;

            existing.Title = Model.Title;
            existing.Description = Model?.Description!;
            existing.Notes = Model?.Notes;
            existing.AnnouncementDate = Model!.AnnouncementDate;
            existing.StatusId = Model.StatusId;
            existing.VisibleToRoleIds = visibleToRoleIds;
            existing.IsActive = Model.IsActive;

            return await repo.UpdateAnnouncementAsync(existing, Email);
        }

        public async Task<bool> ArchiveAnnouncementByIdAsync(Guid AnnouncementId, string Email)
        {
            var repo = new AnnouncementRepository();
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(Email);
            var announcement = await repo.GetAnnouncementById(AnnouncementId);
            if (announcement == null || currentUser == null)
                return false;

            announcement.StatusId = (int)AnnouncementStatus.AnnouncementArchived;
            announcement.UpdatedAt = DateTime.Now.SaDateTime();
            announcement.UpdatedBy = currentUser.Id;

            await repo.ArchiveAnnouncementByIdAsync(announcement);
            return true;
        }

        public async Task<bool> DeleteAnnouncementByIdAsync(Guid AnnouncementId, string Email)
        {
            var repo = new AnnouncementRepository();
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(Email);
            var announcement = await repo.GetAnnouncementById(AnnouncementId);

            if (announcement == null || currentUser == null)
                return false;

            announcement.IsDeleted = true;
            announcement.UpdatedAt = DateTime.Now.SaDateTime();
            announcement.UpdatedBy = currentUser.Id;
            announcement.IsActive = false;

            await repo.DeleteAnnouncementByIdAsync(announcement);
            return true;
        }

        public async Task<EditAnnouncementViewModel> PopulateEditAnnouncementModel(AnnouncementViewModel Model)
        {
            var selectedRoleIds = string.IsNullOrWhiteSpace(Model.VisibleToRoleIds)? new List<int>()
            : Model.VisibleToRoleIds
                .Split(',')
                .Where(x => int.TryParse(x.Trim(), out _))
                .Select(x => int.Parse(x.Trim()))
                .ToList();

            return new EditAnnouncementViewModel
            {
                Id = Model.Id,
                Title = Model.Title,
                Description = Model.Description,
                Notes = Model.Notes,
                AnnouncementDate = Model.AnnouncementDate,
                StatusId = Model.StatusId,
                IsActive = Model.IsActive,
                VisibleToRoleIds = Model.VisibleToRoleIds,
                SelectedRoleIds = selectedRoleIds
            };
        }

        public async Task<CreateAnnouncementViewModel> InitiateCreateAnnouncementModel(string Email)
        {
            var model = new CreateAnnouncementViewModel();
            model.CreatedByEmail = Email;
            model.AnnouncementDeliveryTypes = new AnnouncementDeliveryTypeBusiness().GetAllAnnouncementDeliveryTypes();
            return model;
        }

        #region  PRIVATE METHODS
        private static bool IsEmailValid(string? email)
        {
            return !string.IsNullOrWhiteSpace(email)
                   && new EmailAddressAttribute().IsValid(email);
        }

        private async Task<Guid> ProcessAnnouncementDeliveryAsync(CreateAnnouncementViewModel Model)
        {
            var deliveryType = (AnnouncementDeliveryTypeEnum)Model.AnnouncementDeliveryType;
      
            switch (deliveryType)
            {
                case AnnouncementDeliveryTypeEnum.InApp:
                case AnnouncementDeliveryTypeEnum.InAppAndEmail:
                    {
                        var announcementId = await CreateInAppAnnouncementAsync(Model);
                        if (deliveryType == AnnouncementDeliveryTypeEnum.InAppAndEmail && announcementId != Guid.Empty)
                            await SendAnnouncementEmailAsync(Model);
                        return announcementId;
                    }
                case AnnouncementDeliveryTypeEnum.Email:
                    await SendAnnouncementEmailAsync(Model);
                    return Guid.Empty;
                case AnnouncementDeliveryTypeEnum.All:
                    {
                        var announcementId = await CreateInAppAnnouncementAsync(Model);
                        if (announcementId != Guid.Empty)
                        {
                            await SendAnnouncementEmailAsync(Model);
                            await CreateAnnouncementNotificationAsync(Model);
                        }
                        return announcementId;
                    }
                default:
                    await CreateAnnouncementNotificationAsync(Model);
                    return Guid.Empty;
            }
        }

        private static async Task<Guid> CreateInAppAnnouncementAsync(CreateAnnouncementViewModel Model)
        {
            var dataModel = ObjectMapper.Mapper.Map<CreateAnnouncementModel>(Model);
            dataModel.CreatedByEmail = Model.CreatedByEmail!;
            return await new AnnouncementRepository().CreateAnnouncementAsync(dataModel);
        }

        private async Task SendAnnouncementEmailAsync(CreateAnnouncementViewModel Model)
        {
            var recipients = await new UserBusiness.UserBusiness().GetEmailsByRoleIdsAsync(Model.SelectedRoleIds);
            if (!recipients.Any())
                return; // nothing to send to; consider logging this case

            var toEmail = AppSettings.GetFSMFromEmail();
            _ = Task.Run(() => new AnnouncementNotification(Model, toEmail, recipients).SendNotificationWithoutQueue());
        }

        private async Task CreateAnnouncementNotificationAsync(CreateAnnouncementViewModel Model)
        {
            await new UserNotificationBusiness().CreateUserAnnouncementNotificationAsync(Model);
        }
        #endregion
    }
}
