using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.EmailQueue;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using System.Linq.Expressions;

namespace FieldServiceManagement.Repository.Repositories;

public class EmailQueueRepository : IDisposable
{
    private DataContext _dbContext;
    private readonly IRepository<EmailQueue> _repository;
    private bool _disposed = false;

    public EmailQueueRepository()
    {
        _dbContext = new DataContext();
        _repository = new RepositoryService<EmailQueue>(_dbContext);
    }

    public EmailQueue GetById(int id) =>
        _dbContext.EmailQueues.FirstOrDefault(x => x.Id == id);

    public void Insert(EmailQueue model) => _repository.Insert(model);
    public void Update(EmailQueue model) => _repository.Update(model);

    public IEnumerable<EmailQueue> Find(
        Expression<Func<EmailQueue, bool>> predicate) =>
        _dbContext.Set<EmailQueue>().Where(predicate);

    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed) { if (disposing) _dbContext?.Dispose(); _disposed = true; }
    }
    ~EmailQueueRepository() => Dispose(false);
}