namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyMedicamentSale
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public DateTimeOffset SoldAt { get; set; }
    public decimal SalePrice { get; set; }
    public int UnitsSold { get; set; }
}