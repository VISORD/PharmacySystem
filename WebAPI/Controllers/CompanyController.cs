using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database.Connection;
using PharmacySystem.WebAPI.Database.Repositories;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Company;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CompanyController : ControllerBase
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAccountRepository _accountRepository;

    public CompanyController(ICompanyRepository companyRepository, IAccountRepository accountRepository)
    {
        _companyRepository = companyRepository;
        _accountRepository = accountRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [Database(isReadOnly: true)] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var company = await _companyRepository.GetAsync(transaction, companyId, cancellationToken);
        return company is not null
            ? Ok(new ItemResponse(CompanyProfileModel.From(company)))
            : NotFound();
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        [Database] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] CompanyProfileModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyEmail(transaction, companyId, model.Email, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var company = model.To(companyId);
        await _companyRepository.UpdateAsync(transaction, company, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyEmail(IDbTransaction transaction, int companyId, string email, CancellationToken cancellationToken)
    {
        return await _accountRepository.IsEmailDuplicatedAsync(transaction, email, companyId, cancellationToken)
            ? BadRequest(new ItemResponse(Error: "The specified email is already used into the system"))
            : null;
    }

    #endregion
}