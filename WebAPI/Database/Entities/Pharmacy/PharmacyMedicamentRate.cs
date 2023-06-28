using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

[Table(nameof(PharmacyMedicamentRate), Schema = "pharmacy")]
[PrimaryKey(nameof(PharmacyId), nameof(MedicamentId), nameof(StartDate), nameof(StopDate))]
public sealed class PharmacyMedicamentRate
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime StopDate { get; set; }

    [Column(TypeName = "MONEY")]
    public decimal RetailPrice { get; set; }

    public Pharmacy Pharmacy { get; set; } = null!;
    public Medicament.Medicament Medicament { get; set; } = null!;
}

public static class PharmacyMedicamentRateExtensions
{
    public static PharmacyMedicamentRate? RetailPrice(this IEnumerable<PharmacyMedicamentRate> rates, DateTime asOfDate)
    {
        return rates.FirstOrDefault(x => x.StartDate <= asOfDate && asOfDate <= x.StopDate);
    }
}