namespace PharmacySystem.WebAPI.Models.Common;

public sealed record ItemsResponse(IEnumerable<object?>? Items = null, string? Error = null);