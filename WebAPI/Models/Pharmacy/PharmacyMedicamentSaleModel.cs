using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentSaleModel
{
    public DateTimeOffset SoldAt { get; init; } = DateTimeOffset.Now;

    [Required]
    [Range(0, 1_000)]
    public int? UnitsSold { get; init; }
}