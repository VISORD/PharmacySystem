using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

[Table(nameof(PharmacyMedicamentSale), Schema = "pharmacy")]
[PrimaryKey(nameof(PharmacyId), nameof(MedicamentId), nameof(SoldAt))]
public sealed class PharmacyMedicamentSale
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public DateTimeOffset SoldAt { get; set; }

    [Column(TypeName = "MONEY")]
    public decimal SalePrice { get; set; }

    public int UnitsSold { get; set; }

    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy Pharmacy { get; set; } = null!;

    [ForeignKey(nameof(MedicamentId))]
    public Medicament.Medicament Medicament { get; set; } = null!;
}