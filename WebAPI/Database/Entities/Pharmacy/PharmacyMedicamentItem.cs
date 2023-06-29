namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyMedicamentItem
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public string MedicamentName { get; set; } = null!;
    public decimal VendorPrice { get; set; }
    public decimal? RetailPrice { get; set; }
    public DateTime? RateStartDate { get; set; }
    public DateTime? RateStopDate { get; set; }
    public int QuantityOnHand { get; set; }
}