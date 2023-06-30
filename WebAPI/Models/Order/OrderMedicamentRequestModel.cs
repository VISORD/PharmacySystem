using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderMedicamentRequestModel
{
    [Range(0, 1_000_000_000)]
    public int? Count { get; init; }
}