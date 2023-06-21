using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace PharmacySystem.WebAPI.Authentication.Claims;

public sealed class ClaimProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.BindingInfo.BindingSource is not null &&
            context.BindingInfo.BindingSource.CanAcceptDataFrom(CustomBindingSources.Claim) &&
            context.Metadata is DefaultModelMetadata metadata)
        {
            var claimAttribute = (FromClaimAttribute?) metadata.Attributes.ParameterAttributes?.FirstOrDefault(x => x is FromClaimAttribute);
            if (claimAttribute is not null)
            {
                return new ClaimBinder(claimAttribute);
            }
        }

        return null;
    }
}