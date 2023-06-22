using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentSaleListItemModel
{
    public DateTimeOffset SoldAt { get; init; }
    public decimal SalePrice { get; init; }
    public int? UnitsSold { get; init; }

    public static PharmacyMedicamentSaleListItemModel From(PharmacyMedicamentSale sale) => new()
    {
        SoldAt = sale.SoldAt,
        SalePrice = sale.SalePrice,
        UnitsSold = sale.UnitsSold,
    };
}