using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentProfileModel
{
    public PharmacyShortModel Pharmacy { get; init; } = null!;
    public MedicamentShortModel Medicament { get; init; } = null!;
    public PharmacyMedicamentRateModel? Rate { get; init; } = null!;
    public int QuantityOnHand { get; init; }

    public static PharmacyMedicamentProfileModel From(PharmacyMedicamentProfile pharmacyMedicament) => new()
    {
        Pharmacy = new PharmacyShortModel
        {
            Id = pharmacyMedicament.PharmacyId,
            Name = pharmacyMedicament.PharmacyName,
            Address = pharmacyMedicament.PharmacyAddress,
        },
        Medicament = new MedicamentShortModel
        {
            Id = pharmacyMedicament.MedicamentId,
            Name = pharmacyMedicament.MedicamentName,
            VendorPrice = pharmacyMedicament.VendorPrice,
        },
        Rate = pharmacyMedicament.RetailPrice is not null
            ? new PharmacyMedicamentRateModel
            {
                RetailPrice = pharmacyMedicament.RetailPrice,
                StartDate = pharmacyMedicament.RateStartDate!.Value.AsStartDate(),
                StopDate = pharmacyMedicament.RateStopDate!.Value.AsStartDate(),
            }
            : null,
        QuantityOnHand = pharmacyMedicament.QuantityOnHand,
    };
}