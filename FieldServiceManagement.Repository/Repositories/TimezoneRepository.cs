using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Timezone;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class TimezoneRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Timezone> _repository;
        private bool _disposed = false;

        public TimezoneRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Timezone>(_dbContext);
        }

        public List<Timezone> GetTimezoneBySearchName(string SearchName)
        {
            SqlParameter[] parameters = [new("@SearchName", SearchName)];
            const string query = "EXEC [dbo].[GetTimezoneBySearchName] @SearchName";
            return [.. _dbContext.Set<Timezone>().FromSqlRaw(query, parameters)];
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

        ~TimezoneRepository() => Dispose(false);
        #endregion
    }
}
