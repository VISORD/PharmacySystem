using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PharmacySystem.WebAPI.Database.Connection;

public static class CustomBindingSources
{
    public static readonly BindingSource DatabaseConnection = new(
        "DatabaseConnection", // ID of our BindingSource, must be unique
        "BindingSource_DatabaseConnection", // Display name
        isGreedy: true, // Marks whether the source is greedy or not
        isFromRequest: false); // Marks if the source is from HTTP Request
}