using System.ComponentModel.DataAnnotations;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyProfileModel
{
    public int? Id { get; init; }

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

    public IDictionary<DayOfWeek, PharmacyWorkingHoursModel> WorkingHours { get; init; } = null!;

    public static PharmacyProfileModel From(Database.Entities.Pharmacy.Pharmacy pharmacy) => new()
    {
        Id = pharmacy.Id,
        Name = pharmacy.Name,
        Email = pharmacy.Email,
        Phone = pharmacy.Phone,
        Address = pharmacy.Address,
        Latitude = pharmacy.Latitude,
        Longitude = pharmacy.Longitude,
        Description = pharmacy.Description,
        WorkingHours = pharmacy.WorkingHours.ToDictionary(x => x.Weekday, PharmacyWorkingHoursModel.From)
    };

    public Database.Entities.Pharmacy.Pharmacy To(int companyId, int? id = null) => new()
    {
        Id = id ?? Id ?? 0,
        CompanyId = companyId,
        Name = Name,
        Email = Email,
        Phone = Phone,
        Address = Address,
        Latitude = Latitude!.Value,
        Longitude = Longitude!.Value,
        Description = Description,
        WorkingHours = WorkingHours.Select(x => new PharmacyWorkingHours
        {
            PharmacyId = id ?? Id ?? 0,
            Weekday = x.Key,
            StartTime = x.Value.StartTime!.Value,
            StopTime = x.Value.StopTime!.Value,
        }).ToList()
    };
}