using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PharmacySystem.WebAPI.Authentication.Claims;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromClaimAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => CustomBindingSources.Claim;
    public string ClaimType { get; }

    public FromClaimAttribute(string claimType)
    {
        ClaimType = claimType;
    }
}