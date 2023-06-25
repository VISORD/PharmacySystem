using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class PharmacyController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public PharmacyController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var query = _databaseContext.Pharmacies
            .Where(x => x.CompanyId == companyId)
            .FilterByRequest(request.Filtering)
            .OrderByRequest(request.Ordering);

        var totalAmount = await query.CountAsync(cancellationToken);
        var pharmacies = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsPagingResponse(
            totalAmount,
            pharmacies.Select(PharmacyItemPagingModel.From)
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var pharmacy = model.To(companyId);

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidatePharmacyName(pharmacy.Name, companyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _databaseContext.Pharmacies.AddAsync(pharmacy, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new ItemResponse(result.Entity.Id));
    }

    [HttpGet("{pharmacyId:int}")]
    public async Task<IActionResult> Get(
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var pharmacy = await _databaseContext.Pharmacies
            .Include(x => x.WorkingHours)
            .SingleOrDefaultAsync(x => x.Id == pharmacyId && x.CompanyId == companyId, cancellationToken);

        return pharmacy is not null
            ? Ok(new ItemResponse(PharmacyProfileModel.From(pharmacy)))
            : NotFound();
    }

    [HttpPut("{pharmacyId:int}")]
    public async Task<IActionResult> Update(
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var pharmacy = model.To(companyId, pharmacyId);

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(companyId, pharmacyId, cancellationToken)
                               ?? await ValidatePharmacyName(pharmacy.Name, companyId, cancellationToken, pharmacyId);
        if (validationResult is not null)
        {
            return validationResult;
        }

        _databaseContext.Pharmacies.Update(pharmacy);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{pharmacyId:int}")]
    public async Task<IActionResult> Delete(
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(companyId, pharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _databaseContext.Pharmacies
            .Where(x => x.Id == pharmacyId)
            .ExecuteDeleteAsync(cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidatePharmacyName(string pharmacyName, int companyId, CancellationToken cancellationToken, int? pharmacyId = null)
    {
        var isDuplicated = await _databaseContext.Pharmacies
            .AsNoTracking()
            .AnyAsync(x => x.Id != pharmacyId && x.Name == pharmacyName && x.CompanyId == companyId, cancellationToken);

        return isDuplicated
            ? BadRequest(new ItemResponse(Error: "The specified pharmacy name is already used into the system"))
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyPharmacyRelation(int companyId, int pharmacyId, CancellationToken cancellationToken)
    {
        var pharmacy = await _databaseContext.Pharmacies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == pharmacyId, cancellationToken);

        return pharmacy?.CompanyId != companyId
            ? NotFound()
            : null;
    }

    #endregion
}