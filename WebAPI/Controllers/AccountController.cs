using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PharmacySystem.WebAPI.Database.Connection;
using PharmacySystem.WebAPI.Database.Entities.Company;
using PharmacySystem.WebAPI.Database.Repositories;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Company;
using ClaimTypes = PharmacySystem.WebAPI.Authentication.Claims.ClaimTypes;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;

    public AccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsCompany(
        [Database(isReadOnly: true)] SqlConnection connection,
        [FromBody] CompanyAccountSignInModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = ValidateAuthenticationStatus();
        if (validationResult is not null)
        {
            return validationResult;
        }

        var company = await _accountRepository.SignInAsync(transaction, model.Email, model.Password, cancellationToken);
        if (company is null)
        {
            return BadRequest(new ItemResponse(Error: "Invalid credentials"));
        }

        return await SignInChallenge(company);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUpAsCompany(
        [Database] SqlConnection connection,
        [FromBody] CompanyAccountSignUpModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = ValidateAuthenticationStatus() ?? await ValidateCompanyEmail(transaction, model.Email, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var company = await _accountRepository.SignUpAsync(transaction, model.Email, model.Name, model.Password, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return await SignInChallenge(company);
    }

    [HttpPost("sign-out")]
    [Authorize]
    public async Task<IActionResult> SignOutAsCompany()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    [NonAction]
    private async Task<IActionResult> SignInChallenge(Company company)
    {
        var now = DateTimeOffset.UtcNow;

        var principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.CompanyId, company.Id.ToString())
        }, CookieAuthenticationDefaults.AuthenticationScheme));

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IssuedUtc = now,
            ExpiresUtc = now.AddSeconds(TimeSpan.FromMinutes(30).TotalSeconds),
            AllowRefresh = true,
            IsPersistent = true
        });

        return NoContent();
    }

    #region Validation

    [NonAction]
    private IActionResult? ValidateAuthenticationStatus()
    {
        return User.Identity?.IsAuthenticated == true
            ? BadRequest(new ItemResponse(Error: "User has been already authenticated"))
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyEmail(IDbTransaction transaction, string email, CancellationToken cancellationToken)
    {
        return await _accountRepository.IsEmailDuplicatedAsync(transaction, email, cancellationToken: cancellationToken)
            ? BadRequest(new ItemResponse(Error: "Specified email is already used"))
            : null;
    }

    #endregion
}