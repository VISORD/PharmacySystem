using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

[Table(nameof(PharmacyWorkingHours), Schema = "pharmacy")]
[PrimaryKey(nameof(PharmacyId), nameof(Weekday))]
public sealed class PharmacyWorkingHours
{
    public int PharmacyId { get; set; }

    [Column(TypeName = "TINYINT")]
    public DayOfWeek Weekday { get; set; }

    public TimeSpan StartTime { get; set; }
    public TimeSpan StopTime { get; set; }
}