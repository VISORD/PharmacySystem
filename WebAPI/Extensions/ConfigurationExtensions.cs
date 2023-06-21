namespace PharmacySystem.WebAPI.Extensions;

public static class ConfigurationExtensions
{
    public static string GetOrThrow(this IConfiguration configuration, string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return configuration[name] ?? throw new InvalidOperationException($"Missing \"{name}\" parameter is required");
    }
}