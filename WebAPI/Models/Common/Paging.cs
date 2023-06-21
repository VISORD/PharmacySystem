using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PharmacySystem.WebAPI.Models.Common;

public sealed record Paging([Required] int? Number, [Required] int? Size)
{
    [JsonIgnore]
    public int? Offset => Number is null || Size is null ? null : Number * Size;
}