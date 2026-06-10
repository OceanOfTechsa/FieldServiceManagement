using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Industry;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class IndustryRepository
    {
        private DataContext _dbContext;
        private readonly IRepository<Industry> _repository;
        private bool _disposed = false;

        public IndustryRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<Industry>(_dbContext);
        }

        public List<Industry> GetIndustryBySearchName(string SearchName)
        {
            SqlParameter[] parameters = { new("@SearchName", SearchName)};
            const string query = "EXEC [dbo].[GetAIndustriesBySearchName] @SearchName";
            return [.. _dbContext.Set<Industry>().FromSqlRaw(query, parameters)];
        }

        public List<IndustryCategory> GetIndustryCategoryBySearchNameAndIndustryId(string SearchName, int IndustryId)
        {
            SqlParameter[] parameters =
            {
                new("@SearchName", SearchName),
                new("@IndustryId", IndustryId)
            };
            const string query = "EXEC [dbo].[GetAIndustryCategoryBySearchNameAndIndustry] @SearchName, @IndustryId";
            return [.. _dbContext.Set<IndustryCategory>().FromSqlRaw(query, parameters)];
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

        ~IndustryRepository() => Dispose(false);
        #endregion
    }
}
