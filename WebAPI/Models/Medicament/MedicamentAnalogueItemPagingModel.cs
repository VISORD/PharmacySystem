using System.Globalization;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentAnalogueItemPagingModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public byte Type { get; init; }
    public decimal VendorPrice { get; init; }
    public string VendorPriceText => VendorPrice.ToString("0.#####", CultureInfo.CurrentCulture);

    public static MedicamentAnalogueItemPagingModel From(Database.Entities.Medicament.Medicament medicament, bool isAnalogue) => new()
    {
        Id = medicament.Id,
        Name = medicament.Name,
        Type = (byte) (isAnalogue ? 1 : 0),
        VendorPrice = medicament.VendorPrice,
    };
}