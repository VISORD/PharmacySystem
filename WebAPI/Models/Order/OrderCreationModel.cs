using System.ComponentModel.DataAnnotations;
using PharmacySystem.WebAPI.Database.Entities.Order;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderCreationModel
{
    [Required]
    public int? PharmacyId { get; init; }

    public Database.Entities.Order.Order To() => new()
    {
        PharmacyId = PharmacyId!.Value,
        Status = OrderStatus.Draft,
        UpdatedAt = DateTimeOffset.Now,
    };
}