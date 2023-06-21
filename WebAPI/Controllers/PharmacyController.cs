using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/pharmacy")]
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
        var query = _databaseContext.Pharmacies.Where(x => x.CompanyId == companyId);

        query = request.Ordering.Aggregate(query, (current, field) => field.IsAscending
            ? current.OrderBy(x => EF.Property<object>(x, field.FieldName))
            : current.OrderByDescending(x => EF.Property<object>(x, field.FieldName)));

        var totalAmount = await query.CountAsync(cancellationToken);
        var pharmacies = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsPagingResponse(
            request.Paging,
            request.Ordering,
            totalAmount,
            pharmacies.Select(PharmacyListItemModel.From)
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

        var validationResult = await ValidatePharmacyName(pharmacy.Name, companyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        pharmacy = _databaseContext.Pharmacies.Add(pharmacy).Entity;

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new ItemResponse(pharmacy.Id));
    }

    [HttpGet("{pharmacyId:int}")]
    public async Task<IActionResult> Get(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        int pharmacyId,
        CancellationToken cancellationToken
    )
    {
        var pharmacy = await _databaseContext.Pharmacies
            .Include(x => x.WorkingHours)
            .SingleOrDefaultAsync(x => x.Id == pharmacyId && x.CompanyId == companyId, cancellationToken);

        return pharmacy is not null
            ? Ok(new ItemResponse(PharmacyProfileModel.From(pharmacy)))
            : NotFound();
    }

    [HttpPut("{pharmacyId:int}")]
    public async Task<IActionResult> Update(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        int pharmacyId,
        [FromBody] PharmacyProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var pharmacy = model.To(companyId, pharmacyId);

        var validationResult = await ValidateCompanyPharmacyRelation(companyId, pharmacyId, cancellationToken)
                               ?? await ValidatePharmacyName(pharmacy.Name, companyId, cancellationToken, pharmacyId);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

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
        var validationResult = await ValidateCompanyPharmacyRelation(companyId, pharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

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
            .SingleOrDefaultAsync(x => x.Id == pharmacyId, cancellationToken: cancellationToken);

        return pharmacy?.CompanyId != companyId
            ? NotFound()
            : null;
    }

    #endregion
}