using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PharmacySystem.WebAPI.Authentication;

public sealed class AuthenticationEvents : CookieAuthenticationEvents
{
    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }

    public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var now = DateTimeOffset.UtcNow;
        if (now > context.Properties.ExpiresUtc)
        {
            context.RejectPrincipal();
        }

        return Task.CompletedTask;
    }
}