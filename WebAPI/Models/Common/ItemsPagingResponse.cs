namespace PharmacySystem.WebAPI.Models.Common;

public sealed record ItemsPagingResponse(
    int TotalAmount,
    IEnumerable<object?> Items
);