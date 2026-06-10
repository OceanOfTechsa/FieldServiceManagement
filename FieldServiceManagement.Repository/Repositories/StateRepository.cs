using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.State;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class StateRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<State> _repository;
        private bool _disposed = false;

        public StateRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<State>(_dbContext);
        }

        public List<State> GetStateBySearchName(string SearchName, int CountryId)
        {
            SqlParameter[] parameters = [new("@SearchName", SearchName), new("@CountryId", CountryId)];
            const string query = "EXEC [dbo].[GetStateBySearchName] @SearchName, @CountryId";
            return [.. _dbContext.Set<State>().FromSqlRaw(query, parameters)];
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

        ~StateRepository() => Dispose(false);
        #endregion
    }
}
