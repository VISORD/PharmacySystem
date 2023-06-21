using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PharmacySystem.WebAPI.Authentication.Claims;

public sealed class ClaimBinder : IModelBinder
{
    private readonly FromClaimAttribute _contextAttribute;

    public ClaimBinder(FromClaimAttribute contextAttribute)
    {
        ArgumentNullException.ThrowIfNull(contextAttribute);

        _contextAttribute = contextAttribute;
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var claim = bindingContext.ActionContext
            .HttpContext
            .User
            .Claims
            .FirstOrDefault(c => string.Equals(c.Type, _contextAttribute?.ClaimType, StringComparison.InvariantCultureIgnoreCase));

        if (claim is not null)
        {
            var value = Convert.ChangeType(claim.Value, bindingContext.ModelType);
            bindingContext.ValidationState.Add(value, new ValidationStateEntry { SuppressValidation = true });
            bindingContext.Result = ModelBindingResult.Success(value);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}