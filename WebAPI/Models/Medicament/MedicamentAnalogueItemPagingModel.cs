using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentAnalogueItemPagingModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public bool IsAnalogue { get; init; }
    public decimal VendorPrice { get; init; }
    public string VendorPriceText => VendorPrice.Format();

    public static MedicamentAnalogueItemPagingModel From(Database.Entities.Medicament.Medicament medicament, bool isAnalogue) => new()
    {
        Id = medicament.Id,
        Name = medicament.Name,
        IsAnalogue = isAnalogue,
        VendorPrice = medicament.VendorPrice,
    };
}