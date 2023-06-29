using PharmacySystem.WebAPI.Database.Entities.Medicament;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentAnalogueItemPagingModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public bool IsAnalogue { get; init; }
    public decimal VendorPrice { get; init; }
    public string VendorPriceText => VendorPrice.Format();

    public static MedicamentAnalogueItemPagingModel From(MedicamentAnalogue medicamentAnalogue) => new()
    {
        Id = medicamentAnalogue.Id,
        IsAnalogue = medicamentAnalogue.IsAnalogue,
        Name = medicamentAnalogue.Name,
        VendorPrice = medicamentAnalogue.VendorPrice,
    };
}