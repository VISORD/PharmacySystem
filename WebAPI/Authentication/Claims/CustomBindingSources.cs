using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PharmacySystem.WebAPI.Authentication.Claims;

public static class CustomBindingSources
{
    public static readonly BindingSource Claim = new(
        "Claim", // ID of our BindingSource, must be unique
        "BindingSource_Claim", // Display name
        isGreedy: true, // Marks whether the source is greedy or not
        isFromRequest: false); // Marks if the source is from HTTP Request
}