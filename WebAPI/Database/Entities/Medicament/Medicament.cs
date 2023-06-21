using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.WebAPI.Database.Entities.Medicament;

[Table("Medicament", Schema = "medicament")]
public sealed class Medicament
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }

    [Column(TypeName = "MONEY")]
    public decimal Price { get; init; }
}