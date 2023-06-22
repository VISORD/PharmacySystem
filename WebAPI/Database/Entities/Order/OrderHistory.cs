using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.WebAPI.Database.Entities.Order;

[Table(nameof(OrderHistory), Schema = "order")]
public sealed class OrderHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Event { get; set; } = null!;

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;
}