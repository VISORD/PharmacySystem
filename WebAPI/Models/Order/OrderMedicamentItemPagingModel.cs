using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderMedicamentItemPagingModel
{
    public string Id { get; init; } = null!;
    public MedicamentShortModel Medicament { get; init; } = null!;
    public int QuantityOnHand { get; init; }
    public int RequestedCount { get; init; }
    public int? ApprovedCount { get; init; }
    public bool IsApproved { get; init; }

    public static OrderMedicamentItemPagingModel From(OrderMedicament orderMedicament) => new()
    {
        Id = $"{orderMedicament.OrderId}:{orderMedicament.MedicamentId}",
        Medicament = MedicamentShortModel.From(orderMedicament.Medicament),
        QuantityOnHand = orderMedicament.Medicament
            .PharmacyMedicaments
            .SingleOrDefault(x => x.PharmacyId == orderMedicament.Order.PharmacyId)?
            .QuantityOnHand ?? 0,
        RequestedCount = orderMedicament.RequestedCount,
        ApprovedCount = orderMedicament.ApprovedCount,
        IsApproved = orderMedicament.IsApproved,
    };
}