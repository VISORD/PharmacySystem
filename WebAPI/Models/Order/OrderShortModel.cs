using PharmacySystem.WebAPI.Database.Entities.Order;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderShortModel
{
    public int OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public static OrderShortModel From(Database.Entities.Order.Order order) => new()
    {
        OrderId = order.Id,
        Status = order.Status,
        UpdatedAt = order.UpdatedAt,
    };
}