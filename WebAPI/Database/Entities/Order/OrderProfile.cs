namespace PharmacySystem.WebAPI.Database.Entities.Order;

public sealed class OrderProfile
{
    public int Id { get; set; }
    public int PharmacyId { get; set; }
    public string PharmacyName { get; set; } = null!;
    public string PharmacyAddress { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public DateTimeOffset? OrderedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}