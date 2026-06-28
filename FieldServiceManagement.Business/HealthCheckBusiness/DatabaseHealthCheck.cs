using FieldServiceManagement.Business.Infrastructure;
using FieldServiceManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FieldServiceManagement.Business.HealthCheckBusiness
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = ServiceProviderAccessor.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                // Connectivity + query round-trip
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await db.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
                sw.Stop();

                // Pending migrations
                var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();

                // Applied migrations count
                var applied = (await db.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();

                var data = new Dictionary<string, object>
                {
                    ["queryRoundTripMs"] = sw.ElapsedMilliseconds,
                    ["provider"] = db.Database.ProviderName ?? "Unknown",
                    ["appliedMigrations"] = applied.Count,
                    ["pendingMigrations"] = pending.Count,
                    ["database"] = db.Database.GetDbConnection().Database
                };

                if (pending.Count > 0)
                    return HealthCheckResult.Degraded(
                        $"Database is reachable but has {pending.Count} pending migration(s) that have not been applied. Schema may be out of date.", null, data);

                if (sw.ElapsedMilliseconds > 500)
                    return HealthCheckResult.Degraded(
                        $"Database is reachable but query response time is elevated at {sw.ElapsedMilliseconds}ms. This may indicate performance issues.", null, data);

                return HealthCheckResult.Healthy(
                    "Database is reachable, responsive, and all migrations have been applied.", data);
            }
            catch (DbUpdateException ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Database is reachable but failed to execute a test query. There may be a schema or permission issue.", ex);
            }
            catch (InvalidOperationException ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Could not resolve the database context. The connection may be misconfigured.", ex);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Could not connect to the database. The server may be down or the connection string may be invalid.", ex);
            }
        }
    }
}