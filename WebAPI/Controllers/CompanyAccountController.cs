using System.Data;
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Database.Entities.Company;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Company;
using ClaimTypes = PharmacySystem.WebAPI.Authentication.Claims.ClaimTypes;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/company")]
public sealed class CompanyAccountController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public CompanyAccountController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignInAsCompany(
        [FromBody] CompanyAccountSignInModel model,
        CancellationToken cancellationToken
    )
    {
        var validationResult = ValidateAuthenticationStatus();
        if (validationResult is not null)
        {
            return validationResult;
        }

        var company = await _databaseContext.Database.GetDbConnection().QuerySingleOrDefaultAsync<Company>(new CommandDefinition($@"
            SELECT
                 [Id]      [{nameof(Company.Id)}]
                ,[Email]   [{nameof(Company.Email)}]
                ,[Name]    [{nameof(Company.Name)}]
                ,[Phone]   [{nameof(Company.Phone)}]
            FROM [company].[Company]
            WHERE [Email] = @{nameof(model.Email)} AND [Password] = @{nameof(model.Password)};
        ", parameters: model, cancellationToken: cancellationToken));

        if (company is null)
        {
            return BadRequest(new ItemResponse(Error: "Invalid credentials"));
        }

        return await SignInChallenge(company);
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUpAsCompany(
        [FromBody] CompanyAccountSignUpModel model,
        CancellationToken cancellationToken
    )
    {
        var validationResult = ValidateAuthenticationStatus()
                               ?? await ValidateCompanyEmail(model, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var company = await transaction.GetDbTransaction().Connection.QuerySingleAsync<Company>(new CommandDefinition($@"
            DECLARE @Id TABLE ([Id] INT);

            INSERT INTO [company].[Company] ([Email], [Name], [Password])
            OUTPUT INSERTED.[Id] INTO @Id
            VALUES (@{nameof(model.Email)}, @{nameof(model.Name)}, @{nameof(model.Password)});

            SELECT
                 c.[Id]      [{nameof(Company.Id)}]
                ,c.[Email]   [{nameof(Company.Email)}]
                ,c.[Name]    [{nameof(Company.Name)}]
                ,c.[Phone]   [{nameof(Company.Phone)}]
            FROM [company].[Company] c
            JOIN @Id i ON i.[Id] = c.[Id];
        ", parameters: model, transaction: transaction.GetDbTransaction(), cancellationToken: cancellationToken));

        await _databaseContext.SaveChangesAsync(cancellationToken);
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
    private async Task<IActionResult?> ValidateCompanyEmail(CompanyAccountSignUpModel model, CancellationToken cancellationToken)
    {
        var isDuplicated = await _databaseContext.Companies
            .AsNoTracking()
            .AnyAsync(x => x.Email == model.Email, cancellationToken);

        return isDuplicated
            ? BadRequest(new ItemResponse(Error: "Specified email is already used"))
            : null;
    }

    #endregion
}