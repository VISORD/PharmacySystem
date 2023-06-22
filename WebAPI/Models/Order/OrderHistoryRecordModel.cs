using PharmacySystem.WebAPI.Database.Entities.Order;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderHistoryRecordModel
{
    public DateTimeOffset Timestamp { get; init; }
    public string Event { get; init; } = null!;

    public static OrderHistoryRecordModel From(OrderHistory record) => new()
    {
        Timestamp = record.Timestamp,
        Event = record.Event,
    };
}