using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderShortModel
{
    public int OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime? OrderedAt { get; init; }
    public string? OrderedAtText => OrderedAt?.FormatDateTime();

    public static OrderShortModel From(Database.Entities.Order.Order order) => new()
    {
        OrderId = order.Id,
        Status = order.Status,
        OrderedAt = order.OrderedAt?.LocalDateTime
    };
}