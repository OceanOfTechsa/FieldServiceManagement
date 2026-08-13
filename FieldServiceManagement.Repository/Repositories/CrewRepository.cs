using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Crew;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FieldServiceManagement.Repository.Repositories
{
    public class CrewRepository
    {
        private readonly DataContext _dbContext;
        private readonly IRepository<Crew> _repository;
        private bool _disposed = false;

        public CrewRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Crew>(_dbContext);
        }

        public async Task<List<CrewListItem>> GetCrewsByOrganisationIdAsync(Guid organisationId)
        {
            var param = new SqlParameter("@OrganisationId", organisationId);
            return await _dbContext.Database
                .SqlQueryRaw<CrewListItem>("EXEC [dbo].[GetCrewsByOrganisationId] @OrganisationId", param)
                .ToListAsync();
        }

        public async Task<Guid?> CreateCrewAsync(CreateCrew model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@CrewSize", (object?)model.CrewSize ?? DBNull.Value),
                new SqlParameter("@Description", (object?)model.Description ?? DBNull.Value),
                new SqlParameter("@CreatedBy", model.CreatedById),
                new SqlParameter("@OrganisationId", model.OrganisationId)
            };

            var query = @"EXEC [dbo].[InsertCrew] @Name, @CrewSize, @Description, @CreatedBy, @OrganisationId";

            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            var row = result.FirstOrDefault();
            return row?.Success == true ? row.Id : null;
        }

        public async Task<CrewFullProfileResult> GetCrewByIdAsync(Guid id, Guid organisationId)
        {
            var crewRows = await GetCrewDetailsByIdAsync(id, organisationId);
            var members = await GetCrewMembersAsync(id, organisationId);

            return new CrewFullProfileResult
            {
                CrewRows = crewRows,
                Members = members
            };
        }

        public async Task<List<CrewMemberResults>> GetCrewMembersAsync(Guid crewId, Guid organisationId)
        {
            var parameters = new[]
            {
                new SqlParameter("@CrewId", crewId),
                new SqlParameter("@OrganisationId", organisationId)
            };

            var query = @"EXEC [dbo].[GetCrewMembers] @CrewId, @OrganisationId";

            return await _dbContext.Database
                .SqlQueryRaw<CrewMemberResults>(query, parameters)
                .ToListAsync();
        }

        public async Task<List<CrewDetails>> GetCrewDetailsByIdAsync(Guid id, Guid organisationId)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@OrganisationId", organisationId)
            };

            var query = @"EXEC [dbo].[GetCrewById] @Id, @OrganisationId";

            return await _dbContext.Database.SqlQueryRaw<CrewDetails>(query, parameters).ToListAsync();
        }

        public async Task<Crew?> GetCrewByIdAsync(Guid id, Guid? organisationId)
        {
            return _repository.Find(x => x.Id == id && x.OrganisationId == organisationId && !x.IsDeleted).FirstOrDefault();
        }

        public async Task<List<CrewSearchResult>> SearchCrewsAsync(string term, Guid organisationId)
        {
            var parameters = new[]
            {
                new SqlParameter("@Term", term),
                new SqlParameter("@OrganisationId", organisationId)
            };

            var query = @"EXEC [dbo].[SearchCrews] @Term, @OrganisationId";

            return await _dbContext.Database
                .SqlQueryRaw<CrewSearchResult>(query, parameters)
                .ToListAsync();
        }

        public async Task<RepoResults> AddCrewMemberAsync(Guid crewId, Guid userId, Guid organisationId, Guid performedBy, bool isLead = false)
        {
            var parameters = new[]
            {
                new SqlParameter("@CrewId", crewId),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@IsLead", isLead),
                new SqlParameter("@OrganisationId", organisationId),
                new SqlParameter("@PerformedBy", performedBy)
            };

            var query = @"EXEC [dbo].[AddCrewMember] @CrewId, @UserId, @IsLead, @OrganisationId, @PerformedBy";
            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            var row = result.FirstOrDefault();
            return row ?? new RepoResults();
        }

        public async Task<RepoResults> RemoveCrewMemberAsync(Guid CrewId, Guid UserId, Guid OrgId, string ModifiedBy)
        {
            var parameters = new[]
            {
                 new SqlParameter("@CrewId", CrewId),
                new SqlParameter("@UserId", UserId),
                new SqlParameter("@OrganisationId", OrgId),
                new SqlParameter("@ModifiedBy", ModifiedBy)
            };

            var query = @"EXEC [dbo].[RemoveCrewMember] @CrewId, @UserId, @OrganisationId, @ModifiedBy";
            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            return result.FirstOrDefault() ?? new RepoResults();
        }

        public async ValueTask<int> GetCrewUsersLimitAsync(Guid CrewId, Guid OrgId)
        {
            return _repository.Find(x => x.Id == CrewId && x.OrganisationId == OrgId).Select(x => x.CrewSize).FirstOrDefault();
        }

        public async ValueTask<int> GetNumberOfUsersInACrew(Guid crewId, Guid orgId)
        {
            return await _dbContext.Set<CrewMember>()
                .CountAsync(cm => cm.CrewId == crewId && cm.OrganisationId == orgId);
        }

        public async Task<RepoResults> ChangeCrewLeadAsync(Guid crewId, Guid userId, Guid organisationId, string modifiedBy)
        {
            var parameters = new[]
            {
               new SqlParameter("@CrewId", crewId),
               new SqlParameter("@UserId", userId),
               new SqlParameter("@OrganisationId", organisationId),
               new SqlParameter("@ModifiedBy", modifiedBy)
            };

            var query = @"EXEC ChangeCrewLead @CrewId, @UserId, @OrganisationId, @ModifiedBy";
            var result = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            return result.FirstOrDefault() ?? new RepoResults();
        }

        public async Task<RepoResults> DeleteCrewAsync(Guid CrewId, Guid OrgId)
        {
            var parameters = new[]
            {
                new SqlParameter("@CrewId", CrewId),
                new SqlParameter("@OrganisationId", OrgId)
            };

            var query = @"EXEC [DeleteCrew]  @CrewId, @OrganisationId";
            var results = await _dbContext.Database.SqlQueryRaw<RepoResults>(query, parameters).ToListAsync();
            return results.FirstOrDefault() ?? new RepoResults();
        }

        public async Task<RepoResults> UpdateCrewAsync(
           Guid crewId,
           string name,
           int? crewSize,
           string? description,
           bool isActive,
           Guid organisationId,
           string modifiedBy)
        {
            var parameters = new[]
            {
                new SqlParameter("@CrewId", crewId),
                new SqlParameter("@Name", (object?)name ?? DBNull.Value),
                new SqlParameter("@CrewSize", SqlDbType.Int) { Value = (object?)crewSize ?? DBNull.Value },
                new SqlParameter("@Description", SqlDbType.NVarChar, -1) { Value = (object?)description ?? DBNull.Value },
                new SqlParameter("@IsActive", isActive),
                new SqlParameter("@OrganisationId", organisationId),
                new SqlParameter("@ModifiedBy", modifiedBy)
            };

            var query = @"EXEC UpdateCrew @CrewId, @Name, @CrewSize, @Description, @IsActive, @OrganisationId, @ModifiedBy";

            var result = await _dbContext.Database
                .SqlQueryRaw<RepoResults>(query, parameters)
                .ToListAsync();

            return result.FirstOrDefault() ?? new RepoResults();
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

        ~CrewRepository() => Dispose(false);
        #endregion
    }
}
