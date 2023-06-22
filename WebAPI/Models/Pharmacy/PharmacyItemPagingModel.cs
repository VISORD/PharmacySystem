namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyItemPagingModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string Address { get; init; } = null!;

    public static PharmacyItemPagingModel From(Database.Entities.Pharmacy.Pharmacy pharmacy) => new()
    {
        Id = pharmacy.Id,
        Name = pharmacy.Name,
        Address = pharmacy.Address,
    };
}