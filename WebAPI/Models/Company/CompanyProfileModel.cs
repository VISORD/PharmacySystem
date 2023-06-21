using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Company;

public sealed class CompanyProfileModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    [StringLength(50)]
    public string Name { get; init; } = null!;

    [Phone]
    [StringLength(32)]
    public string? Phone { get; init; }

    public static CompanyProfileModel From(Database.Entities.Company.Company company) => new()
    {
        Email = company.Email,
        Name = company.Name,
        Phone = company.Phone,
    };

    public Database.Entities.Company.Company To(int companyId) => new()
    {
        Id = companyId,
        Email = Email,
        Name = Name,
        Phone = Phone,
    };
}