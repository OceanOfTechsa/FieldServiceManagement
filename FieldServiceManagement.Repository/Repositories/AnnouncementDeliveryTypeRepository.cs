using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Announcement;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;

namespace FieldServiceManagement.Repository.Repositories
{
    public class AnnouncementDeliveryTypeRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<AnnouncementDeliveryType> _repository;
        private bool _disposed = false;

        public AnnouncementDeliveryTypeRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<AnnouncementDeliveryType>(_dbContext);
        }

        public List<AnnouncementDeliveryType> GetAllAnnouncementDeliveryTypes()
        {
            return _repository.GetAll().ToList();
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
        ~AnnouncementDeliveryTypeRepository() => Dispose(false);
        #endregion

    }
}