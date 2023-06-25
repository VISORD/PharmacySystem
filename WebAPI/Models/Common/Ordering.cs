using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Common;

public sealed record Ordering([Required] string Field, short? Order = null)
{
    public bool? IsAscending => Order switch
    {
        -1 => false,
        1 => true,
        _ => null
    };
}