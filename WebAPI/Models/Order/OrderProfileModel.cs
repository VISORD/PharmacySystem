using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderProfileModel
{
    public int Id { get; init; }
    public PharmacyShortModel Pharmacy { get; init; } = null!;
    public OrderStatus Status { get; init; }
    public DateTime? OrderedAt { get; init; }
    public string? OrderedAtText => OrderedAt?.FormatDateTime();
    public DateTime UpdatedAt { get; init; }
    public string UpdatedAtText => UpdatedAt.FormatDateTime();

    public static OrderProfileModel From(OrderProfile order) => new()
    {
        Id = order.Id,
        Pharmacy = new PharmacyShortModel
        {
            Id = order.PharmacyId,
            Name = order.PharmacyName,
            Address = order.PharmacyAddress,
        },
        Status = order.Status,
        OrderedAt = order.OrderedAt?.LocalDateTime,
        UpdatedAt = order.UpdatedAt.LocalDateTime,
    };
}