using System.Linq.Expressions;
using FieldServiceManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    // Loads reference data for any import: companies, states, countries, and whatever other modules need later.
    // Nothing in here knows about Contacts.
    public class ImportLookupRepository : IDisposable
    {
        private DataContext _dbContext;
        private bool _disposed = false;

        public ImportLookupRepository()
        {
            _dbContext = DataContext.Create();
        }

        // Reads a table and returns it as a list of (Id, Name).
        //   TEntity : the table, for example Company
        //   TId     : the type of its Id, for example Guid or int
        //   Filter  : which rows to read (null = all rows), for example only this organisation's
        //   GetId / GetName : which properties are the Id and the name
        public async Task<List<(TId Id, string Name)>> GetLookupAsync<TEntity, TId>(
            Expression<Func<TEntity, bool>>? Filter,
            Func<TEntity, TId> GetId,
            Func<TEntity, string> GetName) where TEntity : class
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsNoTracking();

            if (Filter is not null)
                query = query.Where(Filter);

            var rows = await query.ToListAsync();

            return rows.Select(r => (GetId(r), GetName(r))).ToList();
        }

        // Reads a table as a dictionary: key -> value. Used for things like "which country does this state belong to".
        public async Task<Dictionary<TKey, TValue>> GetMapAsync<TEntity, TKey, TValue>(
            Expression<Func<TEntity, bool>>? Filter,
            Func<TEntity, TKey> GetKey,
            Func<TEntity, TValue> GetValue) where TEntity : class where TKey : notnull
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsNoTracking();

            if (Filter is not null)
                query = query.Where(Filter);

            var rows = await query.ToListAsync();

            return rows.ToDictionary(GetKey, GetValue);
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

        ~ImportLookupRepository() => Dispose(false);

        #endregion
    }
}