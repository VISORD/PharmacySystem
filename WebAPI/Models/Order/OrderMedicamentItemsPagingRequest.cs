using PharmacySystem.WebAPI.Models.Common;

namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderMedicamentItemsPagingRequest : ItemsPagingRequestBase
{
    public DateTime AsOfDate { get; init; } = DateTime.Today;
}