namespace PharmacySystem.WebAPI.Database.Publisher;

/// <summary>
/// In test purpose only!
/// </summary>
public sealed class DatabasePublicationStartupFilter : IStartupFilter
{
    private readonly IServiceScopeFactory _factory;

    public DatabasePublicationStartupFilter(IServiceScopeFactory factory)
    {
        _factory = factory;
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

        next(builder);
    };
}