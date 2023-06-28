using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentSaleItemPagingModel
{
    public string Id { get; init; } = null!;
    public DateTime SoldAt { get; init; }
    public string SoldAtText => SoldAt.FormatDateTime();
    public decimal SalePrice { get; init; }
    public string SalePriceText => SalePrice.Format();
    public int UnitsSold { get; init; }

    public static PharmacyMedicamentSaleItemPagingModel From(PharmacyMedicamentSale sale) => new()
    {
        Id = $"{sale.PharmacyId}:{sale.MedicamentId}",
        SoldAt = sale.SoldAt.LocalDateTime,
        SalePrice = sale.SalePrice,
        UnitsSold = sale.UnitsSold,
    };
}