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

    public static OrderMedicamentItemPagingModel From(OrderMedicamentItem orderMedicament) => new()
    {
        Id = $"{orderMedicament.OrderId}:{orderMedicament.MedicamentId}",
        Medicament = new MedicamentShortModel
        {
            Id = orderMedicament.MedicamentId,
            Name = orderMedicament.MedicamentName,
            VendorPrice = orderMedicament.VendorPrice,
        },
        QuantityOnHand = orderMedicament.QuantityOnHand,
        RequestedCount = orderMedicament.RequestedCount,
        ApprovedCount = orderMedicament.ApprovedCount,
        IsApproved = orderMedicament.IsApproved,
    };
}