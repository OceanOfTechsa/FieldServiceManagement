using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.SubscriptionPlan;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;

namespace FieldServiceManagement.Repository.Repositories
{
    public class SubscriptionPlanRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<SubscriptionPlan> _repository;
        private bool _disposed = false;

        public SubscriptionPlanRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<SubscriptionPlan>(_dbContext);
        }

        public async Task<SubscriptionPlan> GetSubscriptionPlanByIdAsync(int PlanId)
        {
            return _repository.GetById(PlanId);
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
        ~SubscriptionPlanRepository() => Dispose(false);
        #endregion
    }
}
