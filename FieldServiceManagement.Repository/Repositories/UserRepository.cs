using FieldServiceManagement.Areas.Identity.Models;
using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Shared;
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

        public Guid GetUserOrganisationIdByUserEmail(string Email)
        {
            return _dbContext.Users
                .Where(u => u.Email!.ToLower() == Email.ToLower())
                .Select(u => u.OrganisationId)
                .FirstOrDefault() ?? Guid.Empty;
        }
        public AppUser? GetByUserName(string Username)
        {
            return _repository.Find(x => x.Email == Username)?.FirstOrDefault();
        }

        public AppUser? GetById(Guid Id)
        {
            return _repository.Find(x => x.Id == Id)?.FirstOrDefault();
        }

        public async Task<List<UserProfileDetails>> GetUserProfileAsync(Guid? UserId, string? Email)
        {
            var parameters = new List<SqlParameter>();

            if (UserId.HasValue)
                parameters.Add(new SqlParameter("@UserId", UserId.Value));
            else
                parameters.Add(new SqlParameter("@UserId", DBNull.Value));

            if (!string.IsNullOrEmpty(Email))
                parameters.Add(new SqlParameter("@Email", Email));
            else
                parameters.Add(new SqlParameter("@Email", DBNull.Value));

            return await _dbContext.Database.SqlQueryRaw<UserProfileDetails>("EXEC [dbo].[GetUserProfile] @UserId, @Email", parameters.ToArray()).ToListAsync();
        }

        public async void Insert(AppUser Model)
        { 
            _repository.Insert(Model);
        }

        public async void UpdateUserAsync(AppUser Model)
        {
            _repository.Update(Model);
        }

        public async Task<Guid?> UpdateUserProfileAsync(AppUser Model, string? UpdatedByEmail, string? IpAddress = null)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", Model.Id),
                new SqlParameter("@Name", Model.Name),
                new SqlParameter("@Surname", (object?)Model.Surname ?? DBNull.Value),
                new SqlParameter("@Email", Model.Email),
                new SqlParameter("@Phone", (object?)Model.Phone ?? DBNull.Value),
                new SqlParameter("@AvatarUrl", (object?)Model.AvatarUrl ?? DBNull.Value),
                new SqlParameter("@UserRoleId", (object?)Model.UserRoleId ?? DBNull.Value),
                new SqlParameter("@PreferredLanguageId", (object?)Model.PreferredLanguageId ?? DBNull.Value),
                new SqlParameter("@IsActive", Model.IsActive),
                new SqlParameter("@StatusId", (object?)Model.StatusId ?? DBNull.Value),
                new SqlParameter("@EmployeeNumber", (object?)Model.EmployeeNumber ?? DBNull.Value),
                new SqlParameter("@SalutationId", Model.SalutationId),
                new SqlParameter("@UpdatedByEmail", UpdatedByEmail!),
                new SqlParameter("@IpAddress", (object?)IpAddress ?? DBNull.Value)
            };

            var query = @"EXEC [dbo].[UpdateUser] @Id, @Name, @Surname, @Email, @Phone, @AvatarUrl, @UserRoleId, @PreferredLanguageId, @IsActive, @StatusId, @EmployeeNumber, @SalutationId, @UpdatedByEmail, @IpAddress";

            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            var row = result.FirstOrDefault();
            if (row?.Success != true)
            {
                throw new InvalidOperationException(
                    $"UpdateUser failed: {row?.ErrorMessage} (line {row?.ErrorLine} in {row?.ErrorProcedure})");
            }
            return row.Id;
        }

        public async Task<AppUser?> InsertAppUser(AppUser Model)
        {
            object[] parameters =
            [
                new SqlParameter("@OrganisationId",      Model.OrganisationId),
                new SqlParameter("@Name",                Model.Name),
                new SqlParameter("@Surname",             Model.Surname),
                new SqlParameter("@Email",               Model.Email),
                new SqlParameter("@Phone",               Model.Phone),
                new SqlParameter("@AvatarUrl",           (object?)Model.AvatarUrl ?? DBNull.Value),
                new SqlParameter("@UserRoleId",          Model.UserRoleId),
                new SqlParameter("@PreferredLanguageId", (object?)Model.PreferredLanguageId ?? DBNull.Value),
                new SqlParameter("@IsActive",            Model.IsActive),
                new SqlParameter("@StatusId",            (object?)Model.StatusId ?? DBNull.Value),
                new SqlParameter("@IsOwner",             Model.IsOwner),
                new SqlParameter("@UserId",              Model.UserId),
                new SqlParameter("@IsDeleted",           Model.IsDeleted),
                new SqlParameter("@CreatedAt",           Model.CreatedAt),
                new SqlParameter("@UpdatedAt",           Model.UpdatedAt),
                new SqlParameter("@CreatedById",         Model.CreatedById),
                new SqlParameter("@UpdatedById",         (object?)Model.UpdatedById ?? DBNull.Value),
                new SqlParameter("@EmployeeNumber",      Model.EmployeeNumber)
            ];

            var results = await _dbContext.AppUsers
                .FromSqlRaw(@"EXEC InsertAppUser @OrganisationId,@Name,@Surname,@Email,@Phone,@AvatarUrl,@UserRoleId,@PreferredLanguageId,@IsActive,@StatusId,@IsOwner,
                @UserId,@IsDeleted,@CreatedAt,@UpdatedAt,@CreatedById,@UpdatedById,@EmployeeNumber", parameters)
                .AsNoTracking()
                .ToListAsync();

            return results.FirstOrDefault();
        }

        public async Task<(Guid UserId, Guid OrganisationId)> CreateUserProfileAndOrganisationAsync(OnboardingViewModel Model, string IdentityUserId, string? IpAddress = null)
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
                new SqlParameter("@UserId",              SqlDbType.UniqueIdentifier) { Value = Guid.Parse(IdentityUserId) },
                new SqlParameter("@Name",                SqlDbType.NVarChar)         { Value = Model.Name },
                new SqlParameter("@Surname",             SqlDbType.NVarChar)         { Value = Model.Surname },
                new SqlParameter("@Email",               SqlDbType.NVarChar)         { Value = Model.Email },
                new SqlParameter("@Phone",               SqlDbType.NVarChar)         { Value = Model.Phone },
                new SqlParameter("@UserRoleId",          SqlDbType.Int)              { Value = (int)UserRole.Administrator },
                new SqlParameter("@PreferredLanguageId", SqlDbType.Int)              { Value = (object?)Model.LanguageId ?? DBNull.Value },
                new SqlParameter("@OrganisationName",    SqlDbType.NVarChar)         { Value = Model.OrganisationName },
                new SqlParameter("@IndustryId",          SqlDbType.Int)              { Value = (object?)Model.IndustryId  ?? DBNull.Value },
                new SqlParameter("@CountryId",           SqlDbType.Int)              { Value = (object?)Model.CountryId   ?? DBNull.Value },
                new SqlParameter("@StateId",             SqlDbType.Int)              { Value = (object?)Model.StateId     ?? DBNull.Value },
                new SqlParameter("@CurrencyId",          SqlDbType.Int)              { Value = (object?)Model.CurrencyId  ?? DBNull.Value },
                new SqlParameter("@TimezoneId",          SqlDbType.Int)              { Value = (object?)Model.TimezoneId  ?? DBNull.Value },
                new SqlParameter("@LanguageId",          SqlDbType.Int)              { Value = (object?)Model.LanguageId  ?? DBNull.Value },
                new SqlParameter("@IpAddress",           SqlDbType.NVarChar)         { Value = (object?)IpAddress ?? DBNull.Value },
                pNewUserId,
                pNewOrganisationId,
            ];

            await _dbContext.Database.ExecuteSqlRawAsync(@"
            EXEC [CreateUserProfileAndOrganisation]
            @UserId, @Name, @Surname, @Email, @Phone,
            @UserRoleId, @PreferredLanguageId,
            @OrganisationName, @IndustryId, @CountryId, @StateId,
            @CurrencyId, @TimezoneId, @LanguageId,
            @IpAddress,
            @NewUserId OUTPUT, @NewOrganisationId OUTPUT",
                parameters);

            return (
                (Guid)pNewUserId.Value,
                (Guid)pNewOrganisationId.Value
            );
        }

        public async Task<List<AppUser>> GetAllUsersByEmailAsync(string Email)
        {
            var param = new SqlParameter("@Email", Email);
            var query = "EXEC [dbo].[GetUsersByOrganisationByEmail] @Email";
            return await _dbContext.Set<AppUser>().FromSqlRaw(query, param).ToListAsync();
        }

        public async Task<int> GetNumberOfUsersByOrganisationId(Guid OrganisationId)
        {
            return _repository.Find(x => x.OrganisationId == OrganisationId).Count();
        }

        public async Task<List<string>> GetEmailsByRoleIdsAsync(List<int> RoleIds)
        {
            if (RoleIds == null || !RoleIds.Any())
                return new List<string>();

            var roleIdsCsv = string.Join(",", RoleIds.Distinct());

            var parameters = new[]
            {
                new SqlParameter("@RoleIds", SqlDbType.NVarChar, -1) { Value = roleIdsCsv }
            };

            var emails = await _dbContext.Database
                .SqlQueryRaw<string>("EXEC dbo.GetUserEmailsByRoleIds @RoleIds", parameters)
                .ToListAsync();

            return emails;
        }

        public async Task<List<UserSearchResult>> SearchUsersAsync(string term, Guid organisationId)
        {
            var parameters = new[]
            {
                new SqlParameter("@Term", term),
                new SqlParameter("@OrganisationId", organisationId)
            };

            var query = @"EXEC [dbo].[SearchUsers] @Term, @OrganisationId";

            return await _dbContext.Database
                .SqlQueryRaw<UserSearchResult>(query, parameters)
                .ToListAsync();
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
