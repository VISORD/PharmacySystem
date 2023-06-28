using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentRateItemPagingModel
{
    public string Id { get; init; } = null!;
    public decimal RetailPrice { get; init; }
    public string RetailPriceText => RetailPrice.Format();
    public DateTime? StartDate { get; init; }
    public string? StartDateText => StartDate?.FormatDate();
    public DateTime? StopDate { get; init; }
    public string? StopDateText => StopDate?.FormatDate();

    public static PharmacyMedicamentRateItemPagingModel From(PharmacyMedicamentRate rate) => new()
    {
        Id = $"{rate.PharmacyId}:{rate.MedicamentId}",
        RetailPrice = rate.RetailPrice,
        StartDate = rate.StartDate.AsStartDate(),
        StopDate = rate.StopDate.AsStopDate()
    };
}