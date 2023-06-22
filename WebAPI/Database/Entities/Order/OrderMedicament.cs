using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PharmacySystem.WebAPI.Database.Entities.Order;

[Table(nameof(OrderMedicament), Schema = "order")]
[PrimaryKey(nameof(OrderId), nameof(MedicamentId))]
public sealed class OrderMedicament
{
    public int OrderId { get; set; }
    public int MedicamentId { get; set; }
    public int RequestedCount { get; set; }
    public int? ApprovedCount { get; set; }
    public bool IsApproved { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    [ForeignKey(nameof(MedicamentId))]
    public Medicament.Medicament Medicament { get; set; } = null!;
}