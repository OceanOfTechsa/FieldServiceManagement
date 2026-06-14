using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.OrganisationSubscription;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;

namespace FieldServiceManagement.Repository.Repositories
{
    public class OrganisationSubscriptionRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<OrganisationSubscription> _repository;
        private bool _disposed = false;

        public OrganisationSubscriptionRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<OrganisationSubscription>(_dbContext);
        }


        public Task<OrganisationSubscription> GetOrganisationSubscriptionByOrgIdAsync(Guid OrgId)
        {
            var entity = _repository.Find(x => x.OrganisationId == OrgId).FirstOrDefault();
            return Task.FromResult(entity);

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

        ~OrganisationSubscriptionRepository() => Dispose(false);
        #endregion
    }
}
