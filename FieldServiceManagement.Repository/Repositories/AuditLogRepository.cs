using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Audits;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;

namespace FieldServiceManagement.Repository.Repositories
{
    public class AuditLogRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<AuditLog> _repository;
        private bool _disposed = false;

        public AuditLogRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<AuditLog>(_dbContext);
        }

        public async Task LogAsync(AuditLog model)
        {
            _repository.Insert(model);
        }

        //public async Task LogManyAsync(IEnumerable<AuditLog> entries)
        //{
        //    _repository.InsertRange(entries);
        //}

        public async Task<List<AuditLog>> GetByEntityAsync(string EntityName, string EntityId, Guid OrgId)
        {
            return _dbContext.AuditLogs
                .Where(x => x.EntityName == EntityName && x.EntityId == EntityId && !x.IsDeleted && x.OrganisationId == OrgId)
                .ToList();
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

        ~AuditLogRepository() => Dispose(false);
        #endregion
    }
}
