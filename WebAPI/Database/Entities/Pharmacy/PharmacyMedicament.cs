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

    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;

    [ForeignKey(nameof(MedicamentId))]
    public Medicament.Medicament Medicament { get; set; } = null!;

    public ICollection<PharmacyMedicamentRate> Rates { get; set; } = null!;

    public PharmacyMedicamentRate? RetailPrice(DateTime asOfDate) => Rates.FirstOrDefault(x => x.StartDate <= asOfDate && asOfDate <= x.StopDate);
}