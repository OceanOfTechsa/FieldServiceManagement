using FieldServiceManagement.Areas.Identity.Models;
using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using FieldServiceManagement.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FieldServiceManagement.Repository.Repositories
{
    public class UserRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<AppUser> _repository;
        private bool _disposed = false;

        public UserRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<AppUser>(_dbContext);
        }


        public AppUser? GetByUserName(string Username)
        {
            return _repository.Find(x => x.Email == Username)?.FirstOrDefault();
        }

        public AppUser? GetById(Guid Id)
        {
            return _repository.Find(x => x.Id == Id)?.FirstOrDefault();
        }

        public async Task<AppUser?> GetUserProfileByUsernameAsync(string username)
        {
            var param = new SqlParameter("@Email", username);
            var query = "EXEC [dbo].[GetUserProfileByEmail] @Email";
            var results = await _dbContext.Set<AppUser>().FromSqlRaw(query, param).ToListAsync();
            return results.FirstOrDefault();
        }

        public async void Insert(AppUser model)
        { 
            _repository.Insert(model);
        }

        public async void UpdateUserAsync(AppUser model)
        {
            _repository.Update(model);
        }

        public async Task<AppUser?> InsertAppUser(AppUser model)
        {
            object[] parameters =
            [
                new SqlParameter("@OrganisationId",      model.OrganisationId),
                new SqlParameter("@Name",                model.Name),
                new SqlParameter("@Surname",             model.Surname),
                new SqlParameter("@Email",               model.Email),
                new SqlParameter("@Phone",               model.Phone),
                new SqlParameter("@AvatarUrl",           (object?)model.AvatarUrl ?? DBNull.Value),
                new SqlParameter("@UserRoleId",          model.UserRoleId),
                new SqlParameter("@PreferredLanguageId", (object?)model.PreferredLanguageId ?? DBNull.Value),
                new SqlParameter("@IsActive",            model.IsActive),
                new SqlParameter("@StatusId",            (object?)model.StatusId ?? DBNull.Value),
                new SqlParameter("@IsOwner",             model.IsOwner),
                new SqlParameter("@UserId",              model.UserId),
                new SqlParameter("@IsDeleted",           model.IsDeleted),
                new SqlParameter("@CreatedAt",           model.CreatedAt),
                new SqlParameter("@UpdatedAt",           model.UpdatedAt),
                new SqlParameter("@CreatedById",         model.CreatedById),
                new SqlParameter("@UpdatedById",         (object?)model.UpdatedById ?? DBNull.Value),
                new SqlParameter("@EmployeeNumber",      model.EmployeeNumber)
            ];

            var results = await _dbContext.AppUsers
                .FromSqlRaw(@"EXEC InsertAppUser @OrganisationId,@Name,@Surname,@Email,@Phone,@AvatarUrl,@UserRoleId,@PreferredLanguageId,@IsActive,@StatusId,@IsOwner,
                @UserId,@IsDeleted,@CreatedAt,@UpdatedAt,@CreatedById,@UpdatedById,@EmployeeNumber", parameters)
                .AsNoTracking()
                .ToListAsync();

            return results.FirstOrDefault();
        }

        public async Task<(Guid UserId, Guid OrganisationId)> CreateUserProfileAndOrganisationAsync(OnboardingViewModel model, string identityUserId, string? ipAddress = null)
        {
            var pNewUserId = new SqlParameter("@NewUserId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };

            var pNewOrganisationId = new SqlParameter("@NewOrganisationId", SqlDbType.UniqueIdentifier)
            {
                Direction = ParameterDirection.Output
            };

            object[] parameters =
            [
                // @UserId is now UniqueIdentifier in the proc — parse the Identity string to Guid
                new SqlParameter("@UserId",              SqlDbType.UniqueIdentifier) { Value = Guid.Parse(identityUserId) },
                new SqlParameter("@Name",                SqlDbType.NVarChar)         { Value = model.Name },
                new SqlParameter("@Surname",             SqlDbType.NVarChar)         { Value = model.Surname },
                new SqlParameter("@Email",               SqlDbType.NVarChar)         { Value = model.Email },
                new SqlParameter("@Phone",               SqlDbType.NVarChar)         { Value = model.Phone },
                new SqlParameter("@UserRoleId",          SqlDbType.Int)              { Value = (int)UserRole.Administrator },
                new SqlParameter("@PreferredLanguageId", SqlDbType.Int)              { Value = (object?)model.LanguageId ?? DBNull.Value },
                new SqlParameter("@OrganisationName",    SqlDbType.NVarChar)         { Value = model.OrganisationName },
                new SqlParameter("@IndustryId",          SqlDbType.Int)              { Value = (object?)model.IndustryId  ?? DBNull.Value },
                new SqlParameter("@CountryId",           SqlDbType.Int)              { Value = (object?)model.CountryId   ?? DBNull.Value },
                new SqlParameter("@StateId",             SqlDbType.Int)              { Value = (object?)model.StateId     ?? DBNull.Value },
                new SqlParameter("@CurrencyId",          SqlDbType.Int)              { Value = (object?)model.CurrencyId  ?? DBNull.Value },
                new SqlParameter("@TimezoneId",          SqlDbType.Int)              { Value = (object?)model.TimezoneId  ?? DBNull.Value },
                new SqlParameter("@LanguageId",          SqlDbType.Int)              { Value = (object?)model.LanguageId  ?? DBNull.Value },
                new SqlParameter("@PlanId",              SqlDbType.Int)              { Value = (int)SubscriptionPlanEnum.Free },
                new SqlParameter("@IpAddress",           SqlDbType.NVarChar)         { Value = (object?)ipAddress ?? DBNull.Value },
                pNewUserId,
                pNewOrganisationId,
            ];

            await _dbContext.Database.ExecuteSqlRawAsync(@"
            EXEC [CreateUserProfileAndOrganisation]
            @UserId, @Name, @Surname, @Email, @Phone,
            @UserRoleId, @PreferredLanguageId,
            @OrganisationName, @IndustryId, @CountryId, @StateId,
            @CurrencyId, @TimezoneId, @LanguageId, @PlanId,
            @IpAddress,
            @NewUserId OUTPUT, @NewOrganisationId OUTPUT",
                parameters);

            return (
                (Guid)pNewUserId.Value,
                (Guid)pNewOrganisationId.Value
            );
        }


        public async Task<List<AppUser>> GetAllUsersByEmailAsync(string email)
        {
            var param = new SqlParameter("@Email", email);
            var query = "EXEC [dbo].[GetUsersByOrganisationByEmail] @Email";
            return await _dbContext.Set<AppUser>().FromSqlRaw(query, param).ToListAsync();
        }

        public async Task<int> GetNumberOfUsersByOrganisationId(Guid OrganisationId)
        {
            return _repository.Find(x => x.OrganisationId == OrganisationId).Count();
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
        ~UserRepository() => Dispose(false);
        #endregion
    }
}
