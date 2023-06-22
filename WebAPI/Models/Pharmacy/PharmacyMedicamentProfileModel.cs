using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentProfileModel
{
    public PharmacyShortModel Pharmacy { get; init; } = null!;
    public MedicamentShortModel Medicament { get; init; } = null!;
    public int QuantityOnHand { get; init; }

    public static PharmacyMedicamentProfileModel From(PharmacyMedicament pharmacyMedicament) => new()
    {
        Pharmacy = PharmacyShortModel.From(pharmacyMedicament.Pharmacy),
        Medicament = MedicamentShortModel.From(pharmacyMedicament.Medicament),
        QuantityOnHand = pharmacyMedicament.QuantityOnHand,
    };
}