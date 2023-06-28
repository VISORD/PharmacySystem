using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

[Table(nameof(PharmacyMedicament), Schema = "pharmacy")]
[PrimaryKey(nameof(PharmacyId), nameof(MedicamentId))]
public sealed class PharmacyMedicament
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public int QuantityOnHand { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;
    public Medicament.Medicament Medicament { get; set; } = null!;

    public ICollection<PharmacyMedicamentRate> Rates { get; set; } = null!;
}