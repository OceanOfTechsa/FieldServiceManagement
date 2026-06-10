using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Data.DataModels.UserProfileAudit;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;

namespace FieldServiceManagement.Repository.Repositories
{
    public class UserProfileAuditsRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<UserProfileAudit> _repository;
        private bool _disposed = false;

        public UserProfileAuditsRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<UserProfileAudit>(_dbContext);
        }

        public async void LogProfileAuditAsync(UserProfileAudit model)
        {
            _repository.Insert(model);
        }
        public async Task<List<UserProfileAudit>> GetAllUserProfileAuditsByUserIdAsync(Guid Id)
        {
            return _repository.Find(x => x.UserId == Id).ToList();
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
        ~UserProfileAuditsRepository() => Dispose(false);
        #endregion
    }
}
