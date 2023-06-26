using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderItemPagingModel
{
    public int Id { get; init; }
    public PharmacyShortModel Pharmacy { get; init; } = null!;
    public int MedicamentItemCount { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime? OrderedAt { get; init; }
    public string? OrderedAtText => OrderedAt?.Format();
    public DateTime UpdatedAt { get; init; }
    public string UpdatedAtText => UpdatedAt.Format();

    public static OrderItemPagingModel From(Database.Entities.Order.Order order) => new()
    {
        Id = order.Id,
        Pharmacy = PharmacyShortModel.From(order.Pharmacy),
        MedicamentItemCount = order.Medicaments.Count,
        Status = order.Status,
        OrderedAt = order.OrderedAt?.LocalDateTime,
        UpdatedAt = order.UpdatedAt.LocalDateTime,
    };
}