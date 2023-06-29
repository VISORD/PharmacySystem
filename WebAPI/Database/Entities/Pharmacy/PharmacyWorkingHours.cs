namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyWorkingHours
{
    public int PharmacyId { get; set; }
    public DayOfWeek Weekday { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan StopTime { get; set; }
}