namespace PharmacySystem.WebAPI.Database.Entities.Order;

public sealed class Order
{
    public int Id { get; set; }
    public int PharmacyId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTimeOffset? OrderedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}