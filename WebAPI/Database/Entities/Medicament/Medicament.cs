using System.ComponentModel.DataAnnotations.Schema;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Entities.Medicament;

[Table(nameof(Medicament), Schema = "medicament")]
public sealed class Medicament
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    [Column(TypeName = "MONEY")]
    public decimal VendorPrice { get; set; }

    public ICollection<PharmacyMedicament> PharmacyMedicaments { get; set; } = null!;
}