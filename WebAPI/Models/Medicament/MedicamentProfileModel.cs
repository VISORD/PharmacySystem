using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentProfileModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public decimal? VendorPrice { get; init; }
    public string VendorPriceText => VendorPrice!.Value.Format();

    public static MedicamentProfileModel From(Database.Entities.Medicament.Medicament medicament) => new()
    {
        Id = medicament.Id,
        Name = medicament.Name,
        Description = medicament.Description,
        VendorPrice = medicament.VendorPrice,
    };
}