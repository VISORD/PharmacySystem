namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentListItemModel
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;

    public static MedicamentListItemModel From(Database.Entities.Medicament.Medicament medicament) => new()
    {
        Id = medicament.Id,
        Name = medicament.Name,
    };
}