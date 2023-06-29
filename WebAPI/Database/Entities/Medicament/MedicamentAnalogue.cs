namespace PharmacySystem.WebAPI.Database.Entities.Medicament;

public sealed class MedicamentAnalogue
{
    public int Id { get; set; }
    public bool IsAnalogue { get; set; }
    public string Name { get; set; } = null!;
    public decimal VendorPrice { get; set; }
}