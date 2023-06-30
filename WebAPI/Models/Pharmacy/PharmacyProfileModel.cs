using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyProfileModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Address { get; init; } = null!;
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public string? Description { get; init; }
    public IDictionary<DayOfWeek, PharmacyWorkingHoursModel> WorkingHours { get; init; } = new Dictionary<DayOfWeek, PharmacyWorkingHoursModel>();

    public static PharmacyProfileModel From(Database.Entities.Pharmacy.Pharmacy pharmacy, IEnumerable<PharmacyWorkingHours> workingHours) => new()
    {
        Id = pharmacy.Id,
        Name = pharmacy.Name,
        Email = pharmacy.Email,
        Phone = pharmacy.Phone,
        Address = pharmacy.Address,
        Latitude = pharmacy.Latitude,
        Longitude = pharmacy.Longitude,
        Description = pharmacy.Description,
        WorkingHours = workingHours.ToDictionary(x => x.Weekday, PharmacyWorkingHoursModel.From)
    };
}