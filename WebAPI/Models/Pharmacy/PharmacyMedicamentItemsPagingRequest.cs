using PharmacySystem.WebAPI.Models.Common;

namespace PharmacySystem.WebAPI.Models.Pharmacy;

public sealed class PharmacyMedicamentItemsPagingRequest : ItemsPagingRequestBase
{
    public DateTime AsOfDate { get; init; } = DateTime.Today;
}