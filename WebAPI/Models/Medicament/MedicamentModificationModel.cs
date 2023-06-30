using System.ComponentModel.DataAnnotations;
using PharmacySystem.WebAPI.Database.Entities.Medicament;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentModificationModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = null!;

    [StringLength(1024)]
    public string? Description { get; init; }

    [Required]
    [Range(0, 1_000_000_000)]
    public decimal? VendorPrice { get; init; }

    public MedicamentModification To() => new()
    {
        Name = Name,
        Description = Description,
        VendorPrice = VendorPrice!.Value,
    };
}