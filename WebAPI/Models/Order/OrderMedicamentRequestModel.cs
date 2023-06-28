using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderMedicamentRequestModel
{
    [Range(0, int.MaxValue)]
    public int? Count { get; init; }
}