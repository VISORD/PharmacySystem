using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Company;

public sealed class CompanyAccountSignInModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;

    [Required]
    [StringLength(255)]
    public string Password { get; init; } = null!;
}