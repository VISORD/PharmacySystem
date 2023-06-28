using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentOrderItemPagingModel
{
    public string Id { get; init; } = null!;
    public int OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime? OrderedAt { get; init; }
    public string? OrderedAtText => OrderedAt?.FormatDateTime();
    public DateTime UpdatedAt { get; init; }
    public string UpdatedAtText => UpdatedAt.FormatDateTime();

    public static PharmacyMedicamentOrderItemPagingModel From(OrderMedicament orderMedicament) => new()
    {
        Id = $"{orderMedicament.Order.PharmacyId}:{orderMedicament.OrderId}:{orderMedicament.MedicamentId}",
        OrderId = orderMedicament.Order.Id,
        Status = orderMedicament.Order.Status,
        OrderedAt = orderMedicament.Order.OrderedAt?.LocalDateTime,
        UpdatedAt = orderMedicament.Order.UpdatedAt.LocalDateTime,
    };
}