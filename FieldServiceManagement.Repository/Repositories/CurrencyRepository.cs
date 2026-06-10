using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Currency;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class CurrencyRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Currency> _repository;
        private bool _disposed = false;

        public CurrencyRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Currency>(_dbContext);
        }

        public List<Currency> GetCurrencyBySearchName(string SearchName)
        {
            SqlParameter[] parameters = [new("@SearchName", SearchName)];
            const string query = "EXEC [dbo].[GetCurrencyBySearchName] @SearchName";
            return [.. _dbContext.Set<Currency>().FromSqlRaw(query, parameters)];
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

        ~CurrencyRepository() => Dispose(false);
        #endregion
    }
}
