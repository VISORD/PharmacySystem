using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentListItemModel
{
    public MedicamentShortModel Medicament { get; init; } = null!;
    public PharmacyMedicamentRateModel? Rate { get; init; }
    public int QuantityOnHand { get; init; }

    public static PharmacyMedicamentListItemModel From(PharmacyMedicament pharmacyMedicament, DateTime asOfDate) => new()
    {
        Medicament = MedicamentShortModel.From(pharmacyMedicament.Medicament),
        Rate = PharmacyMedicamentRateModel.From(pharmacyMedicament.RetailPrice(asOfDate)),
        QuantityOnHand = pharmacyMedicament.QuantityOnHand,
    };
}