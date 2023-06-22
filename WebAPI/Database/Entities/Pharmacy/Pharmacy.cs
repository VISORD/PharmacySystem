using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

[Table(nameof(Pharmacy), Schema = "pharmacy")]
public sealed class Pharmacy
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Address { get; set; } = null!;

    [Column(TypeName = "DECIMAL(9, 6)")]
    public decimal Latitude { get; set; }

    [Column(TypeName = "DECIMAL(9, 6)")]
    public decimal Longitude { get; set; }

    public string? Description { get; set; }
    public ICollection<PharmacyWorkingHours> WorkingHours { get; set; } = null!;
}