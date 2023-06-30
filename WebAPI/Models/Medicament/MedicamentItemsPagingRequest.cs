using PharmacySystem.WebAPI.Models.Common;

namespace PharmacySystem.WebAPI.Models.Medicament;

public sealed class MedicamentItemsPagingRequest : ItemsPagingRequestBase
{
    public int? ExcludeById { get; init; }
}