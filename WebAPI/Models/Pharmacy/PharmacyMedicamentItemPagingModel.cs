using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentItemPagingModel
{
    public string Id { get; init; } = null!;
    public MedicamentShortModel Medicament { get; init; } = null!;
    public PharmacyMedicamentRateModel? Rate { get; init; }
    public int QuantityOnHand { get; init; }

    public static PharmacyMedicamentItemPagingModel From(PharmacyMedicamentItem pharmacyMedicament) => new()
    {
        Id = $"{pharmacyMedicament.PharmacyId}:{pharmacyMedicament.MedicamentId}",
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