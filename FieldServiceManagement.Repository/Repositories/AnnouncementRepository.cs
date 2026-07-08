using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Announcement;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class AnnouncementRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Announcement> _repository;
        private bool _disposed = false;

        public AnnouncementRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Announcement>(_dbContext);
        }

        public async Task<Announcement> GetAnnouncementById(Guid AnnouncementId)
        {
            return _repository.GetById(AnnouncementId);
        }
        public async Task<List<UserAnnouncement>> GetAnnouncementsForUserAsync(string email)
        {
            object[] parameters =
            [
                new SqlParameter("@Email", email)
            ];

            const string query = "EXEC [dbo].[GetAnnouncementsForUser] @Email";

            return await _dbContext.Set<UserAnnouncement>()
                .FromSqlRaw(query, parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> MarkAnnouncementAsSeenAsync(Guid announcementId, string email)
        {
            object[] parameters =
            [
                new SqlParameter("@AnnouncementId", announcementId),
                new SqlParameter("@Email",           email)
            ];
            const string query = "EXEC [dbo].[MarkAnnouncementAsSeen] @AnnouncementId, @Email";

            var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(query, parameters);
            return rowsAffected >= 0;
        }


        public async Task<int> GetUnseenAnnouncementCountAsync(string email)
        {
            object[] parameters =
            [
                new SqlParameter("@Email", email)
            ];

            const string query = "EXEC [dbo].[GetUnseenAnnouncementCount] @Email";

            var result = await _dbContext.Database
                .SqlQueryRaw<int>(query, parameters)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<Guid> CreateAnnouncementAsync(CreateAnnouncementModel model)
        {
            object[] parameters =
            [
                new SqlParameter("@Title",            model.Title),
                new SqlParameter("@Description",      model.Description),
                new SqlParameter("@Notes",            (object?)model.Notes ?? DBNull.Value),
                new SqlParameter("@AnnouncementDate", model.AnnouncementDate),
                new SqlParameter("@VisibleToRoleIds", (object?)model.VisibleToRoleIds ?? DBNull.Value),
                new SqlParameter("@StatusId",         model.StatusId),
                new SqlParameter("@IsActive",         model.IsActive),
                new SqlParameter("@CreatedByEmail",   model.CreatedByEmail)
            ];

            const string query = "EXEC [dbo].[InsertAnnouncement] @Title,@Description,@Notes,@AnnouncementDate,@VisibleToRoleIds,@StatusId,@IsActive,@CreatedByEmail";

            var result = await _dbContext.Database
                .SqlQueryRaw<Guid>(query, parameters)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<List<AnnouncementAdminListItem>> GetAllAnnouncementsForAdminAsync()
        {
            const string query = "EXEC [dbo].[GetAllAnnouncementsForAdmin]";

            return await _dbContext.Set<AnnouncementAdminListItem>()
                .FromSqlRaw(query)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AnnouncementDetail> GetAnnouncementDetailByIdAsync(Guid AnnouncementId)
        {
            var parameters = new[]
            {
                new SqlParameter("@AnnouncementId", AnnouncementId)
            };
            const string query = "EXEC [dbo].[GetAnnouncementById] @AnnouncementId";
            return _dbContext.Database.SqlQueryRaw<AnnouncementDetail>(query, parameters).AsEnumerable().FirstOrDefault();
        }

        public async Task<bool> UpdateAnnouncementAsync(Announcement announcement, string email)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id",               announcement.Id),
                new SqlParameter("@Title",            announcement.Title),
                new SqlParameter("@Description",      announcement.Description      ?? (object)DBNull.Value),
                new SqlParameter("@Notes",            announcement.Notes            ?? (object)DBNull.Value),
                new SqlParameter("@AnnouncementDate", announcement.AnnouncementDate),
                new SqlParameter("@StatusId",         announcement.StatusId),
                new SqlParameter("@VisibleToRoleIds", announcement.VisibleToRoleIds ?? (object)DBNull.Value),
                new SqlParameter("@IsActive",         announcement.IsActive),
                new SqlParameter("@UpdatedByEmail",   email)
            };

            await _dbContext.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[UpdateAnnouncement] @Id, @Title, @Description, @Notes, @AnnouncementDate, @StatusId, @VisibleToRoleIds, @IsActive, @UpdatedByEmail",
                parameters);
            return true;
        }

        public async Task<bool> ArchiveAnnouncementByIdAsync(Announcement Model)
        {
            _repository.Update(Model);
            return true;
        }

        public async Task<bool> DeleteAnnouncementByIdAsync(Announcement Model)
        {
            _repository.Update(Model);
            return true;
        }

        #region DISPOSE
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }
                _disposed = true;
            }
        }
        ~AnnouncementRepository() => Dispose(false);
        #endregion
    }
}