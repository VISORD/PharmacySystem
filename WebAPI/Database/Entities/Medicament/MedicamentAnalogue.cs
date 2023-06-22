using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmacySystem.WebAPI.Database.Entities.Medicament;

[Table(nameof(MedicamentAnalogue), Schema = "medicament")]
[PrimaryKey(nameof(OriginalId), nameof(AnalogueId))]
public sealed class MedicamentAnalogue
{
    public int OriginalId { get; set; }
    public int AnalogueId { get; set; }

    [ForeignKey(nameof(OriginalId))]
    public Medicament Original { get; set; } = null!;

    [ForeignKey(nameof(AnalogueId))]
    public Medicament Analogue { get; set; } = null!;
}