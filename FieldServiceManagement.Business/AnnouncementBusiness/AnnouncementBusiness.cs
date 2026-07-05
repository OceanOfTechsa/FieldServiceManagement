using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Data.DataModels.Announcement;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Announcement;

namespace FieldServiceManagement.Business.AnnouncementBusiness
{
    public class AnnouncementBusiness
    {
        public async Task<AnnouncementViewModel> GetAnnouncementById(Guid Announcement)
        {
            var dbModel = await new AnnouncementRepository().GetAnnouncementById(Announcement);
            return ObjectMapper.Mapper.Map<AnnouncementViewModel>(dbModel);
        }
        public async Task<AnnouncementListViewModel> GetAnnouncementsForUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new AnnouncementListViewModel();
            }

            var dbModel = await new AnnouncementRepository().GetAnnouncementsForUserAsync(email);
            var announcements = ObjectMapper.Mapper.Map<List<UserAnnouncementViewModel>>(dbModel);
            var viewModel = new AnnouncementListViewModel
            {
                Announcements = announcements
            };

            return viewModel;
        }


        public async Task<bool> MarkAnnouncementAsSeenAsync(Guid announcementId, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            await new AnnouncementRepository().MarkAnnouncementAsSeenAsync(announcementId, email);
            return true;
        }

        public async Task<int> GetUnseenAnnouncementCountAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return 0;
            }

            return await new AnnouncementRepository().GetUnseenAnnouncementCountAsync(email);
        }

        public async Task<Guid> CreateAnnouncementAsync(CreateAnnouncementViewModel model, string createdByEmail)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Title))
                return Guid.Empty;
         
            var dataModel = ObjectMapper.Mapper.Map<CreateAnnouncementModel>(model);
            dataModel.CreatedByEmail = createdByEmail;

            return await new AnnouncementRepository().CreateAnnouncementAsync(dataModel);
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
        public async Task<bool> UpdateAnnouncementAsync(EditAnnouncementViewModel model, string email)
        {
            var repo = new AnnouncementRepository();
            var existing = await repo.GetAnnouncementById(model.Id);

            if (existing == null)
                return false;

            var visibleToRoleIds = model.SelectedRoleIds != null && model.SelectedRoleIds.Any()
                ? string.Join(",", model.SelectedRoleIds)
                : null;

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Notes = model.Notes;
            existing.AnnouncementDate = model.AnnouncementDate;
            existing.StatusId = model.StatusId;
            existing.VisibleToRoleIds = visibleToRoleIds;
            existing.IsActive = model.IsActive;

            return await repo.UpdateAnnouncementAsync(existing, email);
        }

        public async Task<bool> ArchiveAnnouncementByIdAsync(Guid AnnouncementId, string Email)
        {
            var repo = new AnnouncementRepository();
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(Email);
            var announcement = await repo.GetAnnouncementById(AnnouncementId);
            if (announcement == null || currentUser == null)
                return false;

            announcement.StatusId = (int)AnnouncementStatus.AnnouncementArchived;
            announcement.UpdatedAt = DateTime.UtcNow;
            announcement.UpdatedBy = currentUser.Id;

            await repo.ArchiveAnnouncementByIdAsync(announcement);
            return true;
        }

        public async Task<bool> DeleteAnnouncementByIdAsync(Guid announcementId, string email)
        {
            var repo = new AnnouncementRepository();
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);
            var announcement = await repo.GetAnnouncementById(announcementId);

            if (announcement == null || currentUser == null)
                return false;

            announcement.IsDeleted = true;
            announcement.UpdatedAt = DateTime.UtcNow;
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
    }
}
