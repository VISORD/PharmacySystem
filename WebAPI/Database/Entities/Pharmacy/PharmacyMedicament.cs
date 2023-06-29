namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyMedicament
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public int QuantityOnHand { get; set; }
}