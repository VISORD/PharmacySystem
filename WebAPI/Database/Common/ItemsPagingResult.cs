namespace PharmacySystem.WebAPI.Database.Common;

public sealed record ItemsPagingResult<TItem>(int TotalAmount, IEnumerable<TItem> Items);