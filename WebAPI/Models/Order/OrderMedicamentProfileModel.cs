using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderMedicamentProfileModel
{
    public OrderShortModel Order { get; init; } = null!;
    public MedicamentShortModel Medicament { get; init; } = null!;
    public int RequestedCount { get; init; }
    public int? ApprovedCount { get; init; }
    public bool IsApproved { get; init; }

    public static OrderMedicamentProfileModel From(OrderMedicament orderMedicament) => new()
    {
        Order = OrderShortModel.From(orderMedicament.Order),
        Medicament = MedicamentShortModel.From(orderMedicament.Medicament),
        RequestedCount = orderMedicament.RequestedCount,
        ApprovedCount = orderMedicament.ApprovedCount,
        IsApproved = orderMedicament.IsApproved,
    };
}