using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.WebAPI.Database.Entities.Company;

[Table("Company", Schema = "company")]
public sealed class Company
{
    public int Id { get; init; }
    public string Email { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Phone { get; init; }
}