using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentItemPagingModel
{
    public string Id { get; init; } = null!;
    public MedicamentShortModel Medicament { get; init; } = null!;
    public PharmacyMedicamentRateModel? Rate { get; init; }
    public int QuantityOnHand { get; init; }

    public static PharmacyMedicamentItemPagingModel From(PharmacyMedicament pharmacyMedicament, DateTime asOfDate) => new()
    {
        Id = $"{pharmacyMedicament.PharmacyId}:{pharmacyMedicament.MedicamentId}",
        Medicament = MedicamentShortModel.From(pharmacyMedicament.Medicament),
        Rate = PharmacyMedicamentRateModel.From(pharmacyMedicament.Rates.RetailPrice(asOfDate)),
        QuantityOnHand = pharmacyMedicament.QuantityOnHand,
    };
}