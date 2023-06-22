using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderMedicamentRequestModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int? Count { get; init; }
}