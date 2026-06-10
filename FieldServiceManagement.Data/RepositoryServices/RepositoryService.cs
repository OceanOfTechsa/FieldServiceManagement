using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace FieldServiceManagement.Data.RepositoryServices
{
    public class RepositoryService<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private IDbContext _context;

        private readonly DbSet<TEntity> _dbSet;

        public RepositoryService(IDbContext context)
        {
            this._context = context;
            this._dbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }
        public TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }
        public int Insert(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Added;
            }

            return _context.SaveChanges();
        }
        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return predicate == null ? throw new ArgumentNullException(nameof(predicate)) : _dbSet.AsNoTracking().Where(predicate);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }
    }
}
