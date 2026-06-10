using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.EmailLog;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FieldServiceManagement.Repository.Repositories;

public class EmailLogRepository : IDisposable
{
    private DataContext _dbContext;
    private readonly IRepository<EmailLog> _repository;
    private bool _disposed = false;

    public EmailLogRepository()
    {
        _dbContext = new DataContext();
        _repository = new RepositoryService<EmailLog>(_dbContext);
    }

    public EmailLog GetById(int id) =>
        _dbContext.EmailLogs.FirstOrDefault(x => x.Id == id);

    public void Insert(EmailLog model) => _repository.Insert(model);
    public void Update(EmailLog model) => _repository.Update(model);

    public IEnumerable<EmailLog> Find(
        Expression<Func<EmailLog, bool>> predicate) =>
        _dbContext.Set<EmailLog>().Where(predicate);

    public Task<int> UpdateEmailTracking(EmailLog model)
    {
        object[] p =
        [
            new SqlParameter("@FormsSubmissionAuditId", model.FormsSubmissionAuditId),
            new SqlParameter("@ReadReceipt",            model.ReadReceipt),
            new SqlParameter("@DateAccessed",           model.DateAccessed),
            new SqlParameter("@UserAgent",              model.UserAgent),
        ];
        return _dbContext.Database.ExecuteSqlRawAsync(
            "EXEC sp_UpdateEmailLog @FormsSubmissionAuditId, @ReadReceipt, @DateAccessed, @UserAgent", p);
    }

    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed) { if (disposing) _dbContext?.Dispose(); _disposed = true; }
    }
    ~EmailLogRepository() => Dispose(false);
}