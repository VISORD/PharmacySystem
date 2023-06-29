using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database.Connection;
using PharmacySystem.WebAPI.Database.Repositories;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MedicamentController : ControllerBase
{
    private readonly IMedicamentRepository _medicamentRepository;
    private readonly IMedicamentAnalogueRepository _medicamentAnalogueRepository;

    public MedicamentController(IMedicamentRepository medicamentRepository, IMedicamentAnalogueRepository medicamentAnalogueRepository)
    {
        _medicamentRepository = medicamentRepository;
        _medicamentAnalogueRepository = medicamentAnalogueRepository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [Database(isReadOnly: true)] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var result = await _medicamentRepository.ListAsync(transaction, companyId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(MedicamentItemPagingModel.From)
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [Database] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var medicament = model.To(companyId);

        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateMedicamentName(transaction, companyId, medicament.Name, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _medicamentRepository.AddAsync(transaction, medicament, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{medicamentId:int}")]
    public async Task<IActionResult> Get(
        [Database(isReadOnly: true)] SqlConnection connection,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(transaction, companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var medicament = await _medicamentRepository.GetAsync(transaction, medicamentId, cancellationToken);
        return medicament is not null
            ? Ok(new ItemResponse(MedicamentProfileModel.From(medicament)))
            : NotFound();
    }

    [HttpPut("{medicamentId:int}")]
    public async Task<IActionResult> Update(
        [Database] SqlConnection connection,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentProfileModel model,
        CancellationToken cancellationToken
    )
    {
        var medicament = model.To(companyId, medicamentId);

        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(transaction, companyId, medicamentId, cancellationToken)
                               ?? await ValidateMedicamentName(transaction, companyId, medicament.Name, cancellationToken, medicamentId);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _medicamentRepository.UpdateAsync(transaction, medicament, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{medicamentId:int}")]
    public async Task<IActionResult> Delete(
        [Database] SqlConnection connection,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(transaction, companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _medicamentRepository.DeleteAsync(transaction, medicamentId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{medicamentId:int}/analogue/list")]
    public async Task<IActionResult> Analogues(
        [Database(isReadOnly: true)] SqlConnection connection,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] MedicamentAnalogueItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(transaction, companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _medicamentAnalogueRepository.ListAsync(transaction, medicamentId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(MedicamentAnalogueItemPagingModel.From)
        ));
    }

    [HttpPut("{medicamentId:int}/analogue/associate")]
    public async Task<IActionResult> Associate(
        [Database] SqlConnection connection,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] IList<int> analogueIds,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(transaction, companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _medicamentAnalogueRepository.AssociateAsync(transaction, medicamentId, analogueIds, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{medicamentId:int}/analogue/disassociate")]
    public async Task<IActionResult> Disassociate(
        [Database] SqlConnection connection,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] IList<int> analogueIds,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyMedicamentRelation(transaction, companyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await _medicamentAnalogueRepository.DisassociateAsync(transaction, medicamentId, analogueIds, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidateMedicamentName(IDbTransaction transaction, int companyId, string medicamentName, CancellationToken cancellationToken, int? medicamentId = null)
    {
        return await _medicamentRepository.IsNameDuplicatedAsync(transaction, companyId, medicamentName, medicamentId, cancellationToken)
            ? BadRequest(new ItemResponse(Error: "The specified medicament name is already used into the system"))
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyMedicamentRelation(IDbTransaction transaction, int companyId, int medicamentId, CancellationToken cancellationToken)
    {
        return !await _medicamentRepository.IsExistAsync(transaction, companyId, medicamentId, cancellationToken)
            ? NotFound()
            : null;
    }

    #endregion
}