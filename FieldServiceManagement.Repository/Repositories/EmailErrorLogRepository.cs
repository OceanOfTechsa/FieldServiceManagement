using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.EmailErrorLog;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using System.Linq.Expressions;

namespace FieldServiceManagement.Repository.Repositories;

public class EmailErrorLogRepository : IDisposable
{
    private DataContext _dbContext;
    private readonly IRepository<EmailErrorLog> _repository;
    private bool _disposed = false;

    public EmailErrorLogRepository()
    {
        _dbContext = new DataContext();
        _repository = new RepositoryService<EmailErrorLog>(_dbContext);
    }

    public EmailErrorLog GetById(int id) =>
        _dbContext.EmailErrorLogs.FirstOrDefault(x => x.Id == id);

    public void Insert(EmailErrorLog model) => _repository.Insert(model);
    public void Update(EmailErrorLog model) => _repository.Update(model);
    public void Delete(EmailErrorLog model) => _repository.Delete(model);

    public IEnumerable<EmailErrorLog> Find(
        Expression<Func<EmailErrorLog, bool>> predicate) =>
        _dbContext.Set<EmailErrorLog>().Where(predicate);

    public IEnumerable<EmailErrorLog> GetAll() =>
        _dbContext.Set<EmailErrorLog>();

    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed) { if (disposing) _dbContext?.Dispose(); _disposed = true; }
    }
    ~EmailErrorLogRepository() => Dispose(false);
}
