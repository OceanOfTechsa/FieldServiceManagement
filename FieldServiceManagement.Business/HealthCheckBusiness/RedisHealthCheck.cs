using CacheManager.Core;
using FieldServiceManagement.Business.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace FieldServiceManagement.Business.HealthCheckBusiness
{
    public class RedisHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = ServiceProviderAccessor.Services.CreateScope();
                var cache = scope.ServiceProvider.GetRequiredService<ICacheManager<object>>();

                // Connectivity + round-trip via a lightweight write/read/delete
                var testKey = "healthcheck:ping";
                var testValue = Guid.NewGuid().ToString();

                var sw = System.Diagnostics.Stopwatch.StartNew();
                cache.Put(testKey, testValue);
                var readBack = cache.Get(testKey);
                cache.Remove(testKey);
                sw.Stop();

                if (readBack?.ToString() != testValue)
                    return HealthCheckResult.Unhealthy(
                        "Redis is reachable but did not return the expected value on read-back. Cache may be misconfigured or evicting unexpectedly.");

                var data = new Dictionary<string, object>
                {
                    ["roundTripMs"] = sw.ElapsedMilliseconds,
                    ["cacheHandles"] = cache.CacheHandles.Count()
                };

                if (sw.ElapsedMilliseconds > 500)
                    return HealthCheckResult.Degraded(
                        $"Redis is reachable but response time is elevated at {sw.ElapsedMilliseconds}ms. This may indicate network latency or server load.", null, data);

                return HealthCheckResult.Healthy(
                    "Redis is reachable, responsive, and read/write operations succeeded.", data);
            }
            catch (RedisConnectionException ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Could not establish a connection to Redis. The server may be down or the connection string may be invalid.", ex);
            }
            catch (RedisTimeoutException ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Redis connection timed out. The server may be overloaded or unreachable.", ex);
            }
            catch (InvalidOperationException ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Could not resolve the cache manager. Redis may be misconfigured.", ex);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "An unexpected error occurred while checking Redis health.", ex);
            }
        }
    }
}