using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

[Table("Pharmacy", Schema = "pharmacy")]
public sealed class Pharmacy
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public string Name { get; init; } = null!;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Address { get; init; } = null!;

    [Column(TypeName = "DECIMAL(3, 6)")]
    public decimal Latitude { get; init; }

    [Column(TypeName = "DECIMAL(3, 6)")]
    public decimal Longitude { get; init; }

    public string? Description { get; init; }
    public IList<PharmacyWorkingHours> WorkingHours { get; init; } = null!;
}