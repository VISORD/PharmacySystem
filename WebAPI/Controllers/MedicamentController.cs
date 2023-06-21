using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MedicamentController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public MedicamentController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = _databaseContext.Medicaments.Where(x => x.CompanyId == companyId);

        query = request.Ordering.Aggregate(query, (current, field) => field.IsAscending
            ? current.OrderBy(x => EF.Property<object>(x, field.FieldName))
            : current.OrderByDescending(x => EF.Property<object>(x, field.FieldName)));

        var totalAmount = await query.CountAsync(cancellationToken);
        var medicaments = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsPagingResponse(
            request.Paging,
            request.Ordering,
            totalAmount,
            medicaments.Select(MedicamentListItemModel.From)
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var medicament = model.To(companyId);

        var validationResult = await ValidateMedicamentName(medicament.Name, companyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        medicament = _databaseContext.Medicaments.Add(medicament).Entity;

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new ItemResponse(medicament.Id));
    }

    [HttpGet("{medicamentId:int}")]
    public async Task<IActionResult> Get(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        int medicamentId,
        CancellationToken cancellationToken
    )
    {
        var medicament = await _databaseContext.Medicaments
            .SingleOrDefaultAsync(x => x.Id == medicamentId && x.CompanyId == companyId, cancellationToken);

        return medicament is not null
            ? Ok(new ItemResponse(MedicamentProfileModel.From(medicament)))
            : NotFound();
    }

    [HttpPut("{medicamentId:int}")]
    public async Task<IActionResult> Update(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        int medicamentId,
        [FromBody] MedicamentProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var medicament = model.To(companyId, medicamentId);

        var validationResult = await ValidateCompanyMedicamentRelation(companyId, medicamentId, cancellationToken)
                               ?? await ValidateMedicamentName(medicament.Name, companyId, cancellationToken, medicamentId);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        _databaseContext.Medicaments.Update(medicament);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{medicamentId:int}")]
    public async Task<IActionResult> Delete(
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await ValidateCompanyMedicamentRelation(companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        await _databaseContext.Medicaments
            .Where(x => x.Id == medicamentId)
            .ExecuteDeleteAsync(cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidateMedicamentName(string medicamentName, int companyId, CancellationToken cancellationToken, int? medicamentId = null)
    {
        var isDuplicated = await _databaseContext.Medicaments
            .AsNoTracking()
            .AnyAsync(x => x.Id != medicamentId && x.Name == medicamentName && x.CompanyId == companyId, cancellationToken);

        return isDuplicated
            ? BadRequest(new ItemResponse(Error: "The specified medicament name is already used into the system"))
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyMedicamentRelation(int companyId, int medicamentId, CancellationToken cancellationToken)
    {
        var medicament = await _databaseContext.Medicaments
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == medicamentId, cancellationToken);

        return medicament?.CompanyId != companyId
            ? NotFound()
            : null;
    }

    #endregion
}