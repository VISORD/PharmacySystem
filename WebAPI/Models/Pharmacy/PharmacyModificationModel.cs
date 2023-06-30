using System.ComponentModel.DataAnnotations;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyModificationModel
{
    [Required]
    [StringLength(50)]
    public string Name { get; init; } = null!;

    [EmailAddress]
    public string? Email { get; init; }

    [Phone]
    [StringLength(32)]
    public string? Phone { get; init; }

    [Required]
    [StringLength(512)]
    public string Address { get; init; } = null!;

    [Required]
    public decimal? Latitude { get; init; }

    [Required]
    public decimal? Longitude { get; init; }

    [StringLength(1024)]
    public string? Description { get; init; }

    public IDictionary<DayOfWeek, PharmacyWorkingHoursModel> WorkingHours { get; init; } = new Dictionary<DayOfWeek, PharmacyWorkingHoursModel>();

    public PharmacyModification To() => new()
    {
        Name = Name,
        Email = Email,
        Phone = Phone,
        Address = Address,
        Latitude = Latitude!.Value,
        Longitude = Longitude!.Value,
        Description = Description
    };

    public IEnumerable<PharmacyWorkingHours> ToWorkingHours(int pharmacyId) => WorkingHours.Select(x => new PharmacyWorkingHours
    {
        PharmacyId = pharmacyId,
        Weekday = x.Key,
        StartTime = x.Value.StartTime!.Value,
        StopTime = x.Value.StopTime!.Value,
    });
}