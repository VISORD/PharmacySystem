namespace PharmacySystem.WebAPI.Options;

public sealed class ApplicationOptions : IApplicationOptions
{
    public string DbConnectionString { get; set; } = null!;
}