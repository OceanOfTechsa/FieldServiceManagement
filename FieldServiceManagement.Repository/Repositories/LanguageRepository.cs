using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Language;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class LanguageRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Language> _repository;
        private bool _disposed = false;

        public LanguageRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Language>(_dbContext);
        }

        public List<Language> GetLanguageBySearchName(string SearchName)
        {
            SqlParameter[] parameters = [new("@SearchName", SearchName)];
            const string query = "EXEC [dbo].[GetLanguagesBySearchName] @SearchName";
            return [.. _dbContext.Set<Language>().FromSqlRaw(query, parameters)];
        }

        public List<Language> GetLanguages()
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

        ~LanguageRepository() => Dispose(false);
        #endregion
    }
}
