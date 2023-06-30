namespace PharmacySystem.WebAPI.Database.Entities.Order;

public sealed class OrderModification
{
    public OrderStatus Status { get; set; }
    public DateTimeOffset? OrderedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}