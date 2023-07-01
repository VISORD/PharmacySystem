using Microsoft.Extensions.Options;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Options;

public sealed class ApplicationOptionsSetup : IConfigureOptions<ApplicationOptions>
{
    private const string DbConnectionString = "Database:ConnectionString";

    private readonly IConfiguration _configuration;

    public ApplicationOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ApplicationOptions options)
    {
        options.DbConnectionString = _configuration["DATABASE_CONNECTION_STRING"] ?? _configuration.GetOrThrow(DbConnectionString);
    }
}