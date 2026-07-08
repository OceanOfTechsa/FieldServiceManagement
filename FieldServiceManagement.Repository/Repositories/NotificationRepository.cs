using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Notification;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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