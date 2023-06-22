using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.WebAPI.Database.Entities.Order;

[Table(nameof(Order), Schema = "order")]
public sealed class Order
{
    public int Id { get; set; }
    public int PharmacyId { get; set; }

    [Column("StatusId", TypeName = "TINYINT")]
    public OrderStatus Status { get; set; }

    public DateTimeOffset? OrderedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    [ForeignKey(nameof(PharmacyId))]
    public Pharmacy.Pharmacy Pharmacy { get; set; } = null!;
}