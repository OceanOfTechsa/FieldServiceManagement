using FieldServiceManagement.Business.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Resend;

namespace FieldServiceManagement.Business.HealthCheckBusiness
{
    public class ResendHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                using var scope = ServiceProviderAccessor.Services.CreateScope();
                var resend = scope.ServiceProvider.GetRequiredService<IResend>();
                var domains = await resend.DomainListAsync(cancellationToken);

                sw.Stop();

                var data = new Dictionary<string, object>
                {
                    ["latencyMs"] = sw.ElapsedMilliseconds,
                    ["domainCount"] = domains.Content?.Count ?? 0
                };

                return HealthCheckResult.Healthy(
                    "Resend API is reachable and accepting requests.", data);
            }
            catch (TaskCanceledException ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Resend API request timed out. The service may be slow or unreachable.", ex);
            }
            catch (HttpRequestException ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Could not establish a connection to the Resend API. Check network connectivity or firewall rules.", ex);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "An unexpected error occurred while contacting the Resend API.", ex);
            }
        }
    }
}