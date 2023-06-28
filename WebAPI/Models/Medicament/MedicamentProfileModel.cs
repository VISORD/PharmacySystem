using System.ComponentModel.DataAnnotations;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentProfileModel
{
    public int Id { get; init; }

    [Required]
    [StringLength(100)]
    public string Name { get; init; } = null!;

    [StringLength(1024)]
    public string? Description { get; init; }

    [Required]
    [Range(0, 1_000_000_000)]
    public decimal? VendorPrice { get; init; }

    public string VendorPriceText => VendorPrice!.Value.Format();

    public static MedicamentProfileModel From(Database.Entities.Medicament.Medicament medicament) => new()
    {
        Id = medicament.Id,
        Name = medicament.Name,
        Description = medicament.Description,
        VendorPrice = medicament.VendorPrice,
    };

    public Database.Entities.Medicament.Medicament To(int companyId, int? id = null) => new()
    {
        Id = id ?? 0,
        CompanyId = companyId,
        Name = Name,
        Description = Description,
        VendorPrice = VendorPrice!.Value,
    };
}