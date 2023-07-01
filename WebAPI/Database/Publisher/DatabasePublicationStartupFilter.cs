using PharmacySystem.WebAPI.Health;

namespace PharmacySystem.WebAPI.Database.Publisher;

/// <summary>
/// In test purpose only!
/// </summary>
public sealed class DatabasePublicationStartupFilter : IStartupFilter
{
    private readonly IServiceScopeFactory _factory;
    private readonly HealthCheckStatus _status;

    public DatabasePublicationStartupFilter(IServiceScopeFactory factory, HealthCheckStatus status)
    {
        _factory = factory;
        _status = status;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => builder =>
    {
        using var scope = _factory.CreateScope();

        var result = scope.ServiceProvider
            .GetRequiredService<IDatabasePublicationService>()
            .Publish(new DatabasePublicationOptions { DacpacFileName = "Database.dacpac" });

        if (!result.Success)
        {
            throw new InvalidOperationException("Failed to publish database project", result.Exception);
        }

        _status.IsReady = true;

        next(builder);
    };
}