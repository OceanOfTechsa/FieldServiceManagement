using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Settings;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using FieldServiceManagement.Repository.Contracts;
using FieldServiceManagement.ViewModels.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;


namespace FieldServiceManagement.Repository.Repositories
{
    public class SettingsRepository : ISettingsRepository, IDisposable
    {
        private DataContext _dbContext = null;
        private readonly IRepository<Settings> _repository;
        private bool _disposed = false;


        public SettingsRepository()
        {
            _dbContext = new DataContext();
            _repository = new RepositoryService<Settings>(_dbContext);
        }
        public List<Settings> GetAll()
        {
            return [.. _repository.GetAll()];
        }


        public async Task<SettingsDetails> GetSearchByList(string searchTerm, int pageIndex, int pageSize)
        {
            var model = new SettingsDetails
            {
                SearchTerm = searchTerm,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            object[] parameters =
            {
                new SqlParameter("@SearchTerm", model.SearchTerm),
                new SqlParameter("@PageIndex", model.PageIndex),
                new SqlParameter("@PageSize", model.PageSize)
            };

            const string query = "EXEC [GetSettings] @SearchTerm, @PageIndex, @PageSize";
            model.settingsDetails = await _dbContext.Set<SettingsDetailsForDisplay>().FromSqlRaw(query, parameters).ToListAsync();
            model.RecordCount = model.settingsDetails.Any() ? model.settingsDetails.First().RecordCount : 0;

            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id)
            };

            var query = @"EXEC [dbo].[DeleteSetting] @Id";
            var result = await _dbContext.Database.SqlQueryRaw<RepoResultsInt>(query, parameters).ToListAsync();
            var row = result.FirstOrDefault();
            return row?.Success == true;
        }


        public Settings GetById(int id)
        {
            return _repository.Find(x => x.Id == id).FirstOrDefault();
        }

        public void Insert(Settings model)
        {
            _repository.Insert(model);
        }

        public void Update(Settings model)
        {

            _repository.Update(model);

        }

        public IEnumerable<Settings> Find(Expression<Func<Settings, bool>> predicate)
        {
            return _dbContext.Set<Settings>().Where(predicate);
        }
        public Settings GetByKey(string key)
        {
            return _repository.Find(x => x.key == key).FirstOrDefault();
        }

        public List<Settings> GetSettingByKey(string key)
        {
            return _repository.Find(x => x.key.ToLower() == key.ToLower()).ToList();
        }

        public List<Settings> GetGrantsAssistantEmailOnSettings()
        {
            const string query = "EXEC [GetGrantsAssistantEmailOnSettings]";
            return _dbContext.Set<Settings>().FromSqlRaw(query).ToList();
        }
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

        ~SettingsRepository()
        {
            Dispose(false);
        }
    }
}
