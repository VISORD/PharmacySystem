namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Address { get; set; } = null!;
}