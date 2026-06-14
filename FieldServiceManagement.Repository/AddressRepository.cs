using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Address;
using FieldServiceManagement.Data.DataModels.Language;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository
{
    public class AddressRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Address> _repository;
        private bool _disposed = false;

        public AddressRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Address>(_dbContext);
        }

        public List<Address> GetAllAddressesByOrganisaId(Guid OrganisationId)
        {
            return [.. _repository.Find(x => x.OrganisationId == OrganisationId & x.IsActive == true & x.IsDeleted == false)];
        }

        public List<Address> GetAddressesBySearchTerm(string SearchTerm, string Email)
        {
            SqlParameter[] parameters = [new("@SearchTerm", SearchTerm),new("@Email", Email)];
            const string query = "EXEC [dbo].[GetAddressesBySearchTerm] @SearchTerm, @Email";
            return [.. _dbContext.Set<Address>().FromSqlRaw(query, parameters)];
        }
        public Task<Address> GetAddressByIdAsync(Guid Id)
        {
            var entity = _repository.Find(x => x.Id == Id).FirstOrDefault();
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

        ~AddressRepository() => Dispose(false);
        #endregion
    }
}
