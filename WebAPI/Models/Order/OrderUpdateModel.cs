namespace PharmacySystem.WebAPI.Models.Order;

public sealed class OrderUpdateModel
{
    public Database.Entities.Order.Order To(int orderId, int pharmacyId) => new()
    {
        Id = orderId,
        PharmacyId = pharmacyId,
        UpdatedAt = DateTimeOffset.Now,
    };
}