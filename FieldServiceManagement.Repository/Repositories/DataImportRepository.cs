using FieldServiceManagement.Data;
using FieldServiceManagement.Data.DataModels.DataImportHistory;
using FieldServiceManagement.Data.Extensions;
using FieldServiceManagement.Data.RepositoryServices;
using FieldServiceManagement.Data.RepositoryServices.Contracts;
using FieldServiceManagement.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FieldServiceManagement.Repository.Repositories
{
    public class DataImportRepository : IDisposable
    {
        private DataContext _dbContext;
        private readonly IRepository<DataImportHistory> _repository;
        private bool _disposed = false;

        public DataImportRepository()
        {
            _dbContext = DataContext.Create();
            _repository = new RepositoryService<DataImportHistory>(_dbContext);
        }

        public async Task<bool> FailStaleImportsAsync(Guid OrganisationId, DateTime Cutoff)
        {
            var staleImports = _repository.Find(h =>
                h.OrganisationId == OrganisationId &&
                ((h.StatusId == DataImportStatus.Queued && h.CreatedAt < Cutoff) ||
                 (h.StatusId == DataImportStatus.Running && h.StartedAt < Cutoff))
            ).ToList();

            if (staleImports.Count == 0)
                return false;

            foreach (var history in staleImports)
            {
                history.StatusId = DataImportStatus.Failed;
                history.Message = "The import did not complete.";
                history.CompletedAt = DateTime.Now.SaDateTime();
            }

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HasActiveImportAsync(Guid OrganisationId)
        {
            return await _dbContext.DataImportHistories
                .AnyAsync(h =>
                    h.OrganisationId == OrganisationId &&
                    (h.StatusId == DataImportStatus.Queued ||
                     h.StatusId == DataImportStatus.Running));
        }

        public async Task<bool> AddAsync(DataImportHistory Model)
        {
            _dbContext.DataImportHistories.Add(Model);

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (
                ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                return false;
            }
        }

        public async Task<List<DataImportHistory>> GetHistoryAsync(
            Guid OrganisationId, string? Module)
        {
            return await _dbContext.DataImportHistories
                .AsNoTracking()
                .Where(h =>
                    h.OrganisationId == OrganisationId &&
                    (Module == null || h.Module == Module))
                .OrderByDescending(h => h.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task<string?> GetErrorFileAsync(Guid Id, Guid OrganisationId)
        {
            return await _dbContext.DataImportHistories
                .AsNoTracking()
                .Where(h =>
                    h.Id == Id &&
                    h.OrganisationId == OrganisationId)
                .Select(h => h.ErrorFilePath)
                .FirstOrDefaultAsync();
        }

        // Sets the row to Running and records when it started
        public async Task MarkRunningAsync(Guid Id, DateTime StartedAt)
        {
            var row = await _dbContext.DataImportHistories.FirstOrDefaultAsync(h => h.Id == Id);
            if (row is null) return;

            row.StatusId = DataImportStatus.Running;
            row.StartedAt = StartedAt;
            await _dbContext.SaveChangesAsync();
        }

        // Sets the final status and the numbers shown on the history page
        public async Task CompleteAsync(Guid Id, DataImportStatus Status, int Created, int Updated, int Skipped,
                                        string? ErrorFilePath, string? Message, DateTime CompletedAt)
        {
            var row = await _dbContext.DataImportHistories.FirstOrDefaultAsync(h => h.Id == Id);
            if (row is null) return;

            row.StatusId = Status;
            row.CreatedCount = Created;
            row.UpdatedCount = Updated;
            row.SkippedCount = Skipped;
            row.ErrorFilePath = ErrorFilePath;
            row.Message = Message;
            row.CompletedAt = CompletedAt;
            await _dbContext.SaveChangesAsync();
        }

        // Used when the app starts: the queue lives in memory, so after a restart nothing can finish these rows
        public async Task FailActiveImportsAsync(string Message)
        {
            var rows = await _dbContext.DataImportHistories
                .Where(h => h.StatusId == DataImportStatus.Queued || h.StatusId == DataImportStatus.Running)
                .ToListAsync();

            foreach (var row in rows)
            {
                row.StatusId = DataImportStatus.Failed;
                row.Message = Message;
                row.CompletedAt = DateTime.Now.SaDateTime();
            }

            if (rows.Count > 0) await _dbContext.SaveChangesAsync();
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

        ~DataImportRepository() => Dispose(false);

        #endregion
    }
}