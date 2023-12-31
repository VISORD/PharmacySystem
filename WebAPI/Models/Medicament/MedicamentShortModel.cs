using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentShortModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal VendorPrice { get; init; }
    public string VendorPriceText => VendorPrice.Format();

    public static MedicamentShortModel From(Database.Entities.Medicament.Medicament medicament) => new()
    {
        Id = medicament.Id,
        Name = medicament.Name,
        VendorPrice = medicament.VendorPrice,
    };
}