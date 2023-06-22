using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.WebAPI.Database.Entities.Company;

[Table(nameof(Company), Schema = "company")]
public sealed class Company
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
}