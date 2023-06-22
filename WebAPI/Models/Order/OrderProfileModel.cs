using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderProfileModel
{
    public PharmacyShortModel Pharmacy { get; init; } = null!;
    public OrderStatus Status { get; init; }
    public DateTime? OrderedAt { get; init; }
    public string? OrderedAtText => OrderedAt?.Format();
    public DateTime UpdatedAt { get; init; }
    public string UpdatedAtText => UpdatedAt.Format();

    public static OrderProfileModel From(Database.Entities.Order.Order order) => new()
    {
        Pharmacy = PharmacyShortModel.From(order.Pharmacy),
        Status = order.Status,
        OrderedAt = order.OrderedAt?.LocalDateTime,
        UpdatedAt = order.UpdatedAt.LocalDateTime,
    };
}