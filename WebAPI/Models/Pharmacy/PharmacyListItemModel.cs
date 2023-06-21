namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyListItemModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Address { get; init; } = null!;

    public static PharmacyListItemModel From(Database.Entities.Pharmacy.Pharmacy pharmacy) => new()
    {
        Id = pharmacy.Id,
        Name = pharmacy.Name,
        Address = pharmacy.Address,
    };
}