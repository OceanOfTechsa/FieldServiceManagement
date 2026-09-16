using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Entity;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace FieldServiceManagement.Repository.Repositories
{
    public class EntityAddressRepository
    {

        private DataContext _dbContext;
        private readonly IRepository<EntityAddress> _repository;
        private bool _disposed = false;

        public EntityAddressRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<EntityAddress>(_dbContext);
        }


        public async Task<EntityAddress?> GetEntityAddressByIdAsync(Guid entityAddressId, Guid organisationId)
        {
            return await _dbContext.EntityAddresses.FirstOrDefaultAsync(ea => ea.Id == entityAddressId && ea.OrganisationId == organisationId && !ea.IsDeleted);
        }

        public async Task<bool> UnlinkEntityAddressAsync(EntityAddress? entityAddress, Guid entityAddressId, Guid organisationId, Guid userId)
        {
            if (entityAddress is null)
                return false;

            entityAddress.IsDeleted = true;
            entityAddress.IsActive = false;
            entityAddress.IsPrimary = false;
            entityAddress.UpdatedById = userId;
            entityAddress.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<RepoResults> RemoveCompanyAddressLink(Guid CompanyId, Guid AddressId)
        {
            var link = _repository
                .Find(ea => ea.EntityId == CompanyId && ea.AddressId == AddressId)
                .FirstOrDefault();

            if (link == null)
                return new RepoResults { Success = false, ErrorMessage = "Address link not found." };

            _repository.Delete(link);
            return new RepoResults { Success = true };
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

        ~EntityAddressRepository() => Dispose(false);
        #endregion
    }
}
