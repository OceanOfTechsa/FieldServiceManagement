using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.Company;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FieldServiceManagement.Repository.Repositories
{
    // Loads reference data for any import: companies, states, countries, and whatever other modules need later.
    // Nothing in here knows about Contacts.
    public class ImportRepository : IDisposable
    {
        private DataContext _dbContext;
        private bool _disposed = false;

        public ImportRepository()
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


        // Reads full records (not just Id and Name), for the records an import may update
        public async Task<List<TEntity>> GetEntitiesAsync<TEntity>(Expression<Func<TEntity, bool>>? Filter) where TEntity : class
        {
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsNoTracking();

            if (Filter is not null)
                query = query.Where(Filter);

            return await query.ToListAsync();
        }

        // Saves a whole batch. One SaveChanges is one database transaction: the whole batch is saved, or none of it.
        // The addresses are added before the records that point to them.
        public async Task SaveBatchAsync(ImportBatch Batch)
        {
            _dbContext.Addresses.AddRange(Batch.NewAddresses);
            _dbContext.Contacts.AddRange(Batch.NewContacts);
            _dbContext.Companies.AddRange(Batch.NewCompanies);
            _dbContext.EntityAddresses.AddRange(Batch.NewLinks);
            _dbContext.Notes.AddRange(Batch.NewNotes);

            _dbContext.Addresses.UpdateRange(Batch.UpdatedAddresses);
            _dbContext.Contacts.UpdateRange(Batch.UpdatedContacts);
            _dbContext.Companies.UpdateRange(Batch.UpdatedCompanies);
            _dbContext.EntityAddresses.UpdateRange(Batch.UpdatedLinks);

            await _dbContext.SaveChangesAsync();
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

        ~ImportRepository() => Dispose(false);

        #endregion
    }
}