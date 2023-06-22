using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentRateListItemModel
{
    public decimal RetailPrice { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? StopDate { get; init; }

    public static PharmacyMedicamentRateListItemModel From(PharmacyMedicamentRate rate) => new()
    {
        RetailPrice = rate.RetailPrice,
        StartDate = rate.StartDate.AsStartDate(),
        StopDate = rate.StopDate.AsStopDate()
    };
}