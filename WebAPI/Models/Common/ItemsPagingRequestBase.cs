namespace PharmacySystem.WebAPI.Models.Common;

public abstract class ItemsPagingRequestBase
{
    public IDictionary<string, Filter> Filtering { get; init; } = new Dictionary<string, Filter>();
    public ICollection<Ordering> Ordering { get; init; } = new List<Ordering>();
    public Paging Paging { get; init; } = new(Number: 0, Size: 10);
}