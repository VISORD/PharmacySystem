using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Company;

public sealed class CompanyAccountSignUpModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    [StringLength(50)]
    public string Name { get; init; } = null!;

    [Required]
    [StringLength(255)]
    public string Password { get; init; } = null!;
}