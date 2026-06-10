using System.Linq.Expressions;

namespace FieldServiceManagement.Data.RepositoryServices.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(object id);
        int Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}
