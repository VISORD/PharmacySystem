namespace PharmacySystem.WebAPI.Database.Entities.Medicament;

public sealed class Medicament
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal VendorPrice { get; set; }
}