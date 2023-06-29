using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentOrderItemPagingModel
{
    public string Id { get; init; } = null!;
    public int OrderId { get; init; }
    public int OrderCount { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime? OrderedAt { get; init; }
    public string? OrderedAtText => OrderedAt?.FormatDateTime();
    public DateTime UpdatedAt { get; init; }
    public string UpdatedAtText => UpdatedAt.FormatDateTime();

    public static PharmacyMedicamentOrderItemPagingModel From(PharmacyMedicamentOrderItem pharmacyMedicamentOrderItem) => new()
    {
        Id = $"{pharmacyMedicamentOrderItem.PharmacyId}:{pharmacyMedicamentOrderItem.OrderId}:{pharmacyMedicamentOrderItem.MedicamentId}",
        OrderId = pharmacyMedicamentOrderItem.OrderId,
        OrderCount = pharmacyMedicamentOrderItem.OrderCount,
        Status = pharmacyMedicamentOrderItem.Status,
        OrderedAt = pharmacyMedicamentOrderItem.OrderedAt?.LocalDateTime,
        UpdatedAt = pharmacyMedicamentOrderItem.UpdatedAt.LocalDateTime,
    };
}