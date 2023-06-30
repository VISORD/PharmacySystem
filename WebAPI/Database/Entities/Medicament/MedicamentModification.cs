namespace PharmacySystem.WebAPI.Database.Entities.Medicament;

public sealed class MedicamentModification
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal VendorPrice { get; set; }
}