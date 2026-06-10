using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FieldServiceManagement.Data.RepositoryServices.Contracts
{
    public interface IDbContext
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
        EntityEntry Entry(object entity);
        void Dispose();
    }
}
