namespace PharmacySystem.WebAPI.Database.Entities.Order;

public sealed class OrderMedicament
{
    public int OrderId { get; set; }
    public int MedicamentId { get; set; }
    public int RequestedCount { get; set; }
    public int? ApprovedCount { get; set; }
    public bool IsApproved { get; set; }
}