using PharmacySystem.WebAPI.Database.Entities.Order;

namespace PharmacySystem.WebAPI.Database.Entities.Pharmacy;

public sealed class PharmacyMedicamentOrderItem
{
    public int PharmacyId { get; set; }
    public int MedicamentId { get; set; }
    public int OrderId { get; set; }
    public int OrderCount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTimeOffset? OrderedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}