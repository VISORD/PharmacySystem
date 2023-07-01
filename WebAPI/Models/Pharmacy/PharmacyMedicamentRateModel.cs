using System.ComponentModel.DataAnnotations;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentRateModel
{
    [Required]
    [Range(0, 1_000_000_000)]
    public decimal? RetailPrice { get; init; }

    public string RetailPriceText => RetailPrice!.Value.Format();

    public DateTime? StartDate { get; init; }
    public string? StartDateText => StartDate?.FormatDate();
    public DateTime? StopDate { get; init; }
    public string? StopDateText => StopDate?.FormatDate();
}