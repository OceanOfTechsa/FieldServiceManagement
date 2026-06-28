using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FieldServiceManagement.Business.HealthCheckBusiness
{
    public class ApplicationHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var process = System.Diagnostics.Process.GetCurrentProcess();
                var memoryMb = GC.GetTotalMemory(false) / 1024 / 1024;
                var uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime();

                var data = new Dictionary<string, object>
                {
                    ["version"] = typeof(ApplicationHealthCheck).Assembly.GetName().Version?.ToString() ?? "Unknown",
                    ["environment"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    ["machineName"] = Environment.MachineName,
                    ["uptime"] = uptime.ToString(@"d\.hh\:mm\:ss"),
                    ["memoryMb"] = memoryMb,
                    ["threadCount"] = process.Threads.Count
                };

                // Warn if memory is unusually high
                if (memoryMb > 1024)
                    return Task.FromResult(HealthCheckResult.Degraded(
                        $"Application is running but memory usage is elevated at {memoryMb} MB. Consider investigating for memory leaks.", null, data));

                return Task.FromResult(HealthCheckResult.Healthy(
                    "Application is running and all core metrics are within normal range.", data));
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "An unexpected error occurred while reading application diagnostics.", ex));
            }
        }
    }
}