namespace PharmacySystem.WebAPI.Models.Common;

public abstract class ItemsPagingRequestBase
{
    public Paging Paging { get; init; } = new(Number: 0, Size: 10);
    public IList<Ordering> Ordering { get; init; } = new List<Ordering>();
}