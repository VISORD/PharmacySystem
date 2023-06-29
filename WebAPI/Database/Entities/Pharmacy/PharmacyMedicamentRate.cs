namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyMedicamentRate
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }
    public decimal RetailPrice { get; set; }
}