using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PharmacySystem.WebAPI.Health;

public class HealthCheckService : IHealthCheck
{
    private readonly HealthCheckStatus _status;

    public HealthCheckService(HealthCheckStatus status)
    {
        _status = status;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        return Task.FromResult(_status.IsReady ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy());
    }
}