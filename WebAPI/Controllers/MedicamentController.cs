using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Database.Entities.Medicament;
using PharmacySystem.WebAPI.Extensions;
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
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var query = _databaseContext.Medicaments
            .Where(x => x.CompanyId == companyId)
            .FilterByRequest(request.Filtering)
            .OrderByRequest(request.Ordering);

        var totalAmount = await query.CountAsync(cancellationToken);
        var medicaments = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsPagingResponse(
            totalAmount,
            medicaments.Select(MedicamentItemPagingModel.From)
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

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateMedicamentName(medicament.Name, companyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _databaseContext.Medicaments.AddAsync(medicament, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new ItemResponse(result.Entity.Id));
    }

    [HttpGet("{medicamentId:int}")]
    public async Task<IActionResult> Get(
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var medicament = await _databaseContext.Medicaments
            .SingleOrDefaultAsync(x => x.Id == medicamentId && x.CompanyId == companyId, cancellationToken);

        return medicament is not null
            ? Ok(new ItemResponse(MedicamentProfileModel.From(medicament)))
            : NotFound();
    }

    [HttpPut("{medicamentId:int}")]
    public async Task<IActionResult> Update(
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var medicament = model.To(companyId, medicamentId);

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(companyId, medicamentId, cancellationToken)
                               ?? await ValidateMedicamentName(medicament.Name, companyId, cancellationToken, medicamentId);
        if (validationResult is not null)
        {
            return validationResult;
        }

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
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _databaseContext.Medicaments
            .Where(x => x.Id == medicamentId)
            .ExecuteDeleteAsync(cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{medicamentId:int}/analogue/list")]
    public async Task<IActionResult> Analogues(
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentAnalogueItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var query = _databaseContext.MedicamentAnalogues
            .Include(x => x.Original)
            .Include(x => x.Analogue)
            .Where(x => x.OriginalId == medicamentId || x.AnalogueId == medicamentId)
            .FilterByRequest(request.Filtering)
            .OrderByRequest(request.Ordering);

        var totalAmount = await query.CountAsync(cancellationToken);
        var medicamentAnalogues = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsPagingResponse(
            totalAmount,
            medicamentAnalogues.Select(x =>
            {
                var isAnalogue = x.OriginalId == medicamentId;
                return MedicamentAnalogueItemPagingModel.From(isAnalogue ? x.Analogue : x.Original, isAnalogue);
            })
        ));
    }

    [HttpPut("{medicamentId:int}/analogue/associate")]
    public async Task<IActionResult> Associate(
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] IList<int> analogueIds,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _databaseContext.MedicamentAnalogues.AddRangeAsync(analogueIds.Select(x => new MedicamentAnalogue
        {
            OriginalId = medicamentId,
            AnalogueId = x
        }), cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{medicamentId:int}/analogue/disassociate")]
    public async Task<IActionResult> Disassociate(
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] IList<int> analogueIds,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _databaseContext.MedicamentAnalogues
            .Where(x => x.OriginalId == medicamentId && analogueIds.Contains(x.AnalogueId))
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