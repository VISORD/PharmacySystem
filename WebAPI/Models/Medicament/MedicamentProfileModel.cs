using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentProfileModel
{
    public int? Id { get; init; }

    [Required]
    [StringLength(100)]
    public string Name { get; init; } = null!;

    [StringLength(1024)]
    public string? Description { get; init; }

    [Required]
    public decimal? Price { get; init; }

    public static MedicamentProfileModel From(Database.Entities.Medicament.Medicament medicament) => new()
    {
        Id = medicament.Id,
        Name = medicament.Name,
        Description = medicament.Description,
        Price = medicament.Price,
    };

    public Database.Entities.Medicament.Medicament To(int companyId, int? id = null) => new()
    {
        Id = id ?? Id ?? 0,
        CompanyId = companyId,
        Name = Name,
        Description = Description,
        Price = Price!.Value,
    };
}