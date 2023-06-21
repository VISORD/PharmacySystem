using System.ComponentModel.DataAnnotations;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyWorkingHoursModel
{
    [Required]
    public TimeSpan? StartTime { get; init; }

    [Required]
    public TimeSpan? StopTime { get; init; }

    public static PharmacyWorkingHoursModel From(PharmacyWorkingHours workingHours) => new()
    {
        StartTime = workingHours.StartTime,
        StopTime = workingHours.StopTime,
    };
}