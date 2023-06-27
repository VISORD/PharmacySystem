using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentOrderItemPagingModel
{
    public int OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime? OrderedAt { get; init; }
    public string? OrderedAtText => OrderedAt?.FormatDateTime();
    public DateTime UpdatedAt { get; init; }
    public string UpdatedAtText => UpdatedAt.FormatDateTime();

    public static PharmacyMedicamentOrderItemPagingModel From(Database.Entities.Order.Order order) => new()
    {
        OrderId = order.Id,
        Status = order.Status,
        OrderedAt = order.OrderedAt?.LocalDateTime,
        UpdatedAt = order.UpdatedAt.LocalDateTime,
    };
}