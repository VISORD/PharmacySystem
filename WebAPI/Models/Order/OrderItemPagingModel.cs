using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderItemPagingModel
{
    public PharmacyShortModel Pharmacy { get; init; } = null!;
    public int MedicamentItemsCount { get; init; }
    public OrderStatus Status { get; init; }
    public DateTimeOffset? OrderedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public static OrderItemPagingModel From(Database.Entities.Order.Order order, IReadOnlyDictionary<int, int> medicamentItemsCounts) => new()
    {
        Pharmacy = PharmacyShortModel.From(order.Pharmacy),
        MedicamentItemsCount = medicamentItemsCounts.TryGetValue(order.Id, out var value) ? value : 0,
        Status = order.Status,
        OrderedAt = order.OrderedAt,
        UpdatedAt = order.UpdatedAt,
    };
}