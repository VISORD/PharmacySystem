using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentProfileModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = null!;

    [StringLength(1024)]
    public string? Description { get; init; }

    [Required]
    public decimal? VendorPrice { get; init; }

    public static MedicamentProfileModel From(Database.Entities.Medicament.Medicament medicament) => new()
    {
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