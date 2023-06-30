using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database.Connection;
using PharmacySystem.WebAPI.Database.Repositories;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PharmacyController : ControllerBase
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IPharmacyWorkingHoursRepository _pharmacyWorkingHoursRepository;

    public PharmacyController(IPharmacyRepository pharmacyRepository, IPharmacyWorkingHoursRepository pharmacyWorkingHoursRepository)
    {
        _pharmacyRepository = pharmacyRepository;
        _pharmacyWorkingHoursRepository = pharmacyWorkingHoursRepository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [Database(isReadOnly: true)] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var result = await _pharmacyRepository.ListAsync(transaction, companyId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(PharmacyItemPagingModel.From)
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [Database] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyModificationModel model,
        CancellationToken cancellationToken
    )
    {
        var pharmacy = model.To();

        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidatePharmacyName(transaction, companyId, pharmacy.Name, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var pharmacyId = await _pharmacyRepository.AddAsync(transaction, companyId, pharmacy, cancellationToken);

        var workingHours = model.ToWorkingHours(pharmacyId);
        await _pharmacyWorkingHoursRepository.MergeAsync(transaction, pharmacyId, workingHours, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Ok(new ItemResponse(new PharmacyCreatedModel { Id = pharmacyId }));
    }

    [HttpGet("{pharmacyId:int}")]
    public async Task<IActionResult> Get(
        [Database(isReadOnly: true)] SqlConnection connection,
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(transaction, companyId, pharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var pharmacy = await _pharmacyRepository.GetAsync(transaction, pharmacyId, cancellationToken);
        if (pharmacy is not null)
        {
            var workingHours = await _pharmacyWorkingHoursRepository.GetAsync(transaction, pharmacyId, cancellationToken);
            return Ok(new ItemResponse(PharmacyProfileModel.From(pharmacy, workingHours)));
        }

        return NotFound();
    }

    [HttpPut("{pharmacyId:int}")]
    public async Task<IActionResult> Update(
        [Database] SqlConnection connection,
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyModificationModel model,
        CancellationToken cancellationToken
    )
    {
        var pharmacy = model.To();

        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(transaction, companyId, pharmacyId, cancellationToken)
                               ?? await ValidatePharmacyName(transaction, companyId, pharmacy.Name, cancellationToken, pharmacyId);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _pharmacyRepository.UpdateAsync(transaction, pharmacyId, pharmacy, cancellationToken);

        var workingHours = model.ToWorkingHours(pharmacyId);
        await _pharmacyWorkingHoursRepository.MergeAsync(transaction, pharmacyId, workingHours, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{pharmacyId:int}")]
    public async Task<IActionResult> Delete(
        [Database] SqlConnection connection,
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(transaction, companyId, pharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _pharmacyRepository.DeleteAsync(transaction, pharmacyId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidatePharmacyName(IDbTransaction transaction, int companyId, string pharmacyName, CancellationToken cancellationToken, int? pharmacyId = null)
    {
        return await _pharmacyRepository.IsNameDuplicatedAsync(transaction, companyId, pharmacyName, pharmacyId, cancellationToken)
            ? BadRequest(new ItemResponse(Error: "The specified pharmacy name is already used into the system"))
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyPharmacyRelation(IDbTransaction transaction, int companyId, int pharmacyId, CancellationToken cancellationToken)
    {
        return !await _pharmacyRepository.IsExistAsync(transaction, companyId, pharmacyId, cancellationToken)
            ? NotFound()
            : null;
    }

    #endregion
}