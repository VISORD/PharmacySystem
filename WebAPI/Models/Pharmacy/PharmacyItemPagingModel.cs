using PharmacySystem.WebAPI.Database.Entities.Pharmacy;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyItemPagingModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Address { get; init; } = null!;

    public static PharmacyItemPagingModel From(PharmacyItem pharmacy) => new()
    {
        Id = pharmacy.Id,
        Name = pharmacy.Name,
        Email = pharmacy.Email,
        Phone = pharmacy.Phone,
        Address = pharmacy.Address,
    };
}