using Microsoft.SqlServer.Dac;

namespace PharmacySystem.WebAPI.Database.Publisher;

public sealed class DatabasePublicationResult
{
    public bool Success { get; }
    public string? Report { get; private init; }
    public string? Script { get; private init; }
    public Exception? Exception { get; private init; }

    private DatabasePublicationResult(bool success)
    {
        Success = success;
    }

    public static DatabasePublicationResult Succeed(PublishResult result) => new(true)
    {
        Report = result.DeploymentReport,
        Script = result.DatabaseScript
    };

    public static DatabasePublicationResult Failed(Exception exception) => new(false)
    {
        Exception = exception
    };
}