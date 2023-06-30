using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderModificationModel
{
    [Required]
    public int? PharmacyId { get; init; }
}