namespace PharmacySystem.WebAPI.Database.Entities.Order;

public sealed class OrderMedicamentItem
{
    public int OrderId { get; set; }
    public int MedicamentId { get; set; }
    public string MedicamentName { get; set; } = null!;
    public decimal VendorPrice { get; set; }
    public int QuantityOnHand { get; set; }
    public int RequestedCount { get; set; }
    public int? ApprovedCount { get; set; }
    public bool IsApproved { get; set; }
}