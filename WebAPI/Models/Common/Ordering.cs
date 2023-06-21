using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Common;

public sealed record Ordering([Required] string FieldName, bool IsAscending = true);