using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.WebAPI.Models.Common;

public sealed record Filter([Required] string Field, object? Value = null, string? MatchMode = null);