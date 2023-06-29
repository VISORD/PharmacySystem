namespace PharmacySystem.WebAPI.Database.Entities.Order;

public sealed class OrderHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Event { get; set; } = null!;
}