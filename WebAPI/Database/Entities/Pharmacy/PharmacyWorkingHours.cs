using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

[Table("PharmacyWorkingHours", Schema = "pharmacy")]
[PrimaryKey(nameof(PharmacyId), nameof(Weekday))]
public sealed class PharmacyWorkingHours
{
    public int PharmacyId { get; init; }

    [Column(TypeName = "TINYINT")]
    public DayOfWeek Weekday { get; init; }

    public TimeSpan StartTime { get; init; }
    public TimeSpan StopTime { get; init; }
}