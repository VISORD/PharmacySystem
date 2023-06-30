namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyModification
{
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Address { get; set; } = null!;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Description { get; set; }
}