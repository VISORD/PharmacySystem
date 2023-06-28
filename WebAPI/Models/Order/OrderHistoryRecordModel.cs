using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Extensions;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderHistoryRecordModel
{
    public DateTime Timestamp { get; init; }
    public string TimestampText => Timestamp.FormatDateTime();
    public string Event { get; init; } = null!;

    public static OrderHistoryRecordModel From(OrderHistory record) => new()
    {
        Timestamp = record.Timestamp.LocalDateTime,
        Event = record.Event,
    };
}