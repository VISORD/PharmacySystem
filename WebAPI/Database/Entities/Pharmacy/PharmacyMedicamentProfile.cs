namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyMedicamentProfile
{
    public int PharmacyId { get; set; }
    public string PharmacyName { get; set; } = null!;
    public string PharmacyAddress { get; set; } = null!;
    public int MedicamentId { get; set; }
    public string MedicamentName { get; set; } = null!;
    public decimal VendorPrice { get; set; }
    public decimal? RetailPrice { get; set; }
    public DateTime? RateStartDate { get; set; }
    public DateTime? RateStopDate { get; set; }
    public int QuantityOnHand { get; set; }
}