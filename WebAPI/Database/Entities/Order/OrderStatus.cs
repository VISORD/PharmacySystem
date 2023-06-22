namespace PharmacySystem.WebAPI.Database.Entities.Order;

public enum OrderStatus : byte
{
    Draft = 0,
    Ordered = 1,
    Shipped = 2,
    Delivered = 3
}