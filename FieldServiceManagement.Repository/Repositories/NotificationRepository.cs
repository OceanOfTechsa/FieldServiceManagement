using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Notification;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FieldServiceManagement.Repository.Repositories
{
    public class NotificationRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Notification> _repository;
        private bool _disposed = false;

        public NotificationRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Notification>(_dbContext);
        }

        public async Task<List<UserNotification>> GetNotificationsForUserAsync(string email, NotificationFilter filter)
        {
            object[] parameters =
            [
                new SqlParameter("@Email",      email),
                new SqlParameter("@IsRead",     (object?)filter?.IsRead ?? DBNull.Value),
                new SqlParameter("@Severity",   (object?)filter?.Severity ?? DBNull.Value),
                new SqlParameter("@Type",       (object?)filter?.Type ?? DBNull.Value),
                new SqlParameter("@DateFrom",   (object?)filter?.DateFrom ?? DBNull.Value),
                new SqlParameter("@DateTo",     (object?)filter?.DateTo ?? DBNull.Value),
                new SqlParameter("@SearchTerm", (object?)filter?.SearchTerm ?? DBNull.Value)
            ];
            const string query = "EXEC [dbo].[GetUserNotifications] @Email,@IsRead,@Severity,@Type,@DateFrom,@DateTo,@SearchTerm";

            return await _dbContext.Set<UserNotification>()
                .FromSqlRaw(query, parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId, string email)
        {
            object[] parameters =
            [
                new SqlParameter("@NotificationId", notificationId),
                new SqlParameter("@Email",          email)
            ];
            const string query = "EXEC [dbo].[MarkNotificationAsRead] @NotificationId, @Email";

            var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(string email)
        {
            object[] parameters =
            [
                new SqlParameter("@Email", email)
            ];
            const string query = "EXEC [dbo].[MarkAllNotificationsAsRead] @Email";

            var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<int> GetUnreadNotificationCountAsync(string email)
        {
            object[] parameters =
            [
                new SqlParameter("@Email", email)
            ];
            const string query = "EXEC [dbo].[GetUnreadNotificationCount] @Email";

            var result = await _dbContext.Database
                .SqlQueryRaw<int>(query, parameters)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<bool> CreateNotificationsBulkAsync(CreateUserNotification model)
        {
            if (model.RoleIds == null || !model.RoleIds.Any())
                return false;

            var roleIdsCsv = string.Join(",", model.RoleIds.Distinct());

            var parameters = new[]
            {
                new SqlParameter("@RoleIds", SqlDbType.NVarChar, -1) { Value = roleIdsCsv },
                new SqlParameter("@OrganisationId", SqlDbType.UniqueIdentifier) { Value = (object)model.OrganisationId ?? DBNull.Value },
                new SqlParameter("@Type", SqlDbType.NVarChar, -1) { Value = model.Type },
                new SqlParameter("@Title", SqlDbType.NVarChar, -1) { Value = model.Title },
                new SqlParameter("@Message", SqlDbType.NVarChar, -1) { Value = model.Message },
                new SqlParameter("@RelatedEntityType", SqlDbType.NVarChar, -1) { Value = (object)model.RelatedEntityType ?? DBNull.Value },
                new SqlParameter("@RelatedEntityId", SqlDbType.UniqueIdentifier) { Value = (object)model.RelatedEntityId ?? DBNull.Value },
                new SqlParameter("@Severity", SqlDbType.TinyInt) { Value = model.Severity },
                new SqlParameter("@ActionText", SqlDbType.NVarChar, 100) { Value = (object)model.ActionText ?? DBNull.Value },
                new SqlParameter("@ActionUrl", SqlDbType.NVarChar, 500) { Value = (object)model.ActionUrl ?? DBNull.Value },
                new SqlParameter("@CreatedById", SqlDbType.UniqueIdentifier) { Value = (object)model.CreatedById ?? DBNull.Value },
               new SqlParameter("@CreatedByEmail", SqlDbType.NVarChar, 500) { Value = (object)model.CreatedByEmail ?? DBNull.Value }
            };

            var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(
                "EXEC dbo.CreateNotificationsBulk @RoleIds, @OrganisationId, @Type, @Title, @Message, " +
                "@RelatedEntityType, @RelatedEntityId, @Severity, @ActionText, @ActionUrl, @CreatedById, @CreatedByEmail",
                parameters);

            return rowsAffected > 0;
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
        ~NotificationRepository() => Dispose(false);
        #endregion
    }
}