namespace PharmacySystem.WebAPI.Models.Common;

public sealed record ItemsPagingResponse(
    Paging Paging,
    IEnumerable<Ordering> Ordering,
    int TotalAmount,
    IEnumerable<object?> Items
);