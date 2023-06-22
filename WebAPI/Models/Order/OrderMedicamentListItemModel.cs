using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderMedicamentListItemModel
{
    public MedicamentShortModel Medicament { get; init; } = null!;
    public int Count { get; init; }
    public bool IsApproved { get; init; }

    public static OrderMedicamentListItemModel From(OrderMedicament orderMedicament) => new()
    {
        Medicament = MedicamentShortModel.From(orderMedicament.Medicament),
        Count = orderMedicament.ApprovedCount ?? orderMedicament.RequestedCount,
        IsApproved = orderMedicament.IsApproved,
    };
}