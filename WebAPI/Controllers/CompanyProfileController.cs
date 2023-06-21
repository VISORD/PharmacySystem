using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Company;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/company/profile")]
[Authorize]
public sealed class CompanyProfileController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public CompanyProfileController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        var company = await _databaseContext.Companies.FindAsync(new object?[] { companyId }, cancellationToken);
        return company is not null
            ? Ok(new ItemResponse(CompanyProfileModel.From(company)))
            : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] CompanyProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await ValidateCompanyEmail(companyId, model, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        _databaseContext.Companies.Update(model.To(companyId));

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyEmail(int companyId, CompanyProfileModel model, CancellationToken cancellationToken)
    {
        var isDuplicated = await _databaseContext.Companies
            .AsNoTracking()
            .AnyAsync(x => x.Id != companyId && x.Email == model.Email, cancellationToken);

        return isDuplicated
            ? BadRequest(new ItemResponse(Error: "The specified email is already used into the system"))
            : null;
    }

    #endregion
}