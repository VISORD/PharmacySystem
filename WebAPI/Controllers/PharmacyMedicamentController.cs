using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Order;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/pharmacy/{pharmacyId:int}/medicament")]
[Authorize]
public sealed class PharmacyMedicamentController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public PharmacyMedicamentController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(companyId, pharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var query = _databaseContext.PharmacyMedicaments
            .Include(x => x.Medicament)
            .Include(x => x.Rates)
            .Where(x => x.PharmacyId == pharmacyId)
            .FilterByRequest(request.Filtering)
            .OrderByRequest(request.Ordering);

        var totalAmount = await query.CountAsync(cancellationToken);
        var pharmacyMedicaments = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsPagingResponse(
            totalAmount,
            pharmacyMedicaments.Select(x => PharmacyMedicamentListItemModel.From(x, request.AsOfDate))
        ));
    }

    [HttpGet("{medicamentId:int}")]
    public async Task<IActionResult> Get(
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var pharmacyMedicament = await _databaseContext.PharmacyMedicaments
            .Include(x => x.Pharmacy)
            .Include(x => x.Medicament)
            .SingleOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.MedicamentId == medicamentId, cancellationToken);

        return pharmacyMedicament is not null
            ? Ok(new ItemResponse(PharmacyMedicamentProfileModel.From(pharmacyMedicament)))
            : NotFound();
    }

    [HttpPost("{medicamentId:int}/rate/list")]
    public async Task<IActionResult> Rates(
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var rates = await _databaseContext.PharmacyMedicamentRates
            .Where(x => x.PharmacyId == pharmacyId && x.MedicamentId == medicamentId)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsResponse(rates.Select(PharmacyMedicamentRateListItemModel.From)));
    }

    [HttpPut("{medicamentId:int}/rate")]
    public async Task<IActionResult> Rate(
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentRateModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var rate = new PharmacyMedicamentRate
        {
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId,
            StartDate = model.StartDate.AsStartDate(),
            StopDate = model.StopDate.AsStopDate(),
            RetailPrice = model.RetailPrice!.Value
        };

        if (rate.StartDate > rate.StopDate)
        {
            return BadRequest(new ItemResponse(Error: "Start date should be earlier than stop date or equal it"));
        }

        var pharmacyMedicament = await _databaseContext.PharmacyMedicaments
            .Include(x => x.Rates)
            .FirstOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.MedicamentId == medicamentId, cancellationToken);

        if (pharmacyMedicament is null)
        {
            await _databaseContext.PharmacyMedicaments.AddAsync(new PharmacyMedicament
            {
                PharmacyId = pharmacyId,
                MedicamentId = medicamentId
            }, cancellationToken);
        }

        var intersectedRates = await _databaseContext.PharmacyMedicamentRates
            .Where(x => x.PharmacyId == pharmacyId && x.MedicamentId == medicamentId && x.StartDate <= rate.StopDate && x.StopDate >= rate.StartDate)
            .ToArrayAsync(cancellationToken);

        var ratesToAdd = new List<PharmacyMedicamentRate>();
        var ratesToDelete = new List<PharmacyMedicamentRate>();
        foreach (var intersectedRate in intersectedRates)
        {
            if (intersectedRate.StartDate < rate.StartDate)
            {
                ratesToAdd.Add(new PharmacyMedicamentRate
                {
                    PharmacyId = pharmacyId,
                    MedicamentId = medicamentId,
                    StartDate = intersectedRate.StartDate,
                    StopDate = rate.StartDate.AddDays(-1),
                    RetailPrice = intersectedRate.RetailPrice
                });
            }

            if (rate.StopDate < intersectedRate.StopDate)
            {
                ratesToAdd.Add(new PharmacyMedicamentRate
                {
                    PharmacyId = pharmacyId,
                    MedicamentId = medicamentId,
                    StartDate = rate.StopDate.AddDays(1),
                    StopDate = intersectedRate.StopDate,
                    RetailPrice = intersectedRate.RetailPrice
                });
            }

            if (intersectedRate.StartDate != rate.StartDate || rate.StopDate != intersectedRate.StopDate)
            {
                ratesToAdd.Add(rate);
                ratesToDelete.Add(intersectedRate);
            }
        }

        if (intersectedRates.Length == 0)
        {
            ratesToAdd.Add(rate);
        }

        _databaseContext.PharmacyMedicamentRates.RemoveRange(ratesToDelete);
        await _databaseContext.PharmacyMedicamentRates.AddRangeAsync(ratesToAdd, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{medicamentId:int}/sale/list")]
    public async Task<IActionResult> Sales(
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var sales = await _databaseContext.PharmacyMedicamentSales
            .Where(x => x.PharmacyId == pharmacyId && x.MedicamentId == medicamentId)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsResponse(sales.Select(PharmacyMedicamentSaleListItemModel.From)));
    }

    [HttpPut("{medicamentId:int}/sale")]
    public async Task<IActionResult> Sale(
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentSaleModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var pharmacyMedicament = await _databaseContext.PharmacyMedicaments
            .Include(x => x.Rates)
            .SingleOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.MedicamentId == medicamentId, cancellationToken);

        if (pharmacyMedicament is not { QuantityOnHand: > 0 })
        {
            return BadRequest(new ItemResponse(Error: "Out of stock"));
        }

        if (pharmacyMedicament.QuantityOnHand - model.UnitsSold < 0)
        {
            return BadRequest(new ItemResponse(Error: "Not enough units to sale"));
        }

        var salePrice = pharmacyMedicament.RetailPrice(model.SoldAt.LocalDateTime);
        if (salePrice is null)
        {
            return BadRequest(new ItemResponse(Error: "No retail price for specified date"));
        }

        await _databaseContext.PharmacyMedicamentSales.AddAsync(new PharmacyMedicamentSale
        {
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId,
            SoldAt = model.SoldAt,
            SalePrice = salePrice.RetailPrice,
            UnitsSold = model.UnitsSold!.Value,
        }, cancellationToken);

        pharmacyMedicament.QuantityOnHand -= model.UnitsSold!.Value;
        _databaseContext.PharmacyMedicaments.Update(pharmacyMedicament);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{medicamentId:int}/order/list")]
    public async Task<IActionResult> Orders(
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var orderMedicaments = await _databaseContext.OrderMedicaments
            .Include(x => x.Order)
            .Where(x => x.Order.PharmacyId == pharmacyId && x.MedicamentId == medicamentId)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsResponse(orderMedicaments.Select(x => OrderShortModel.From(x.Order))));
    }

    #region Validation

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

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyPharmacyMedicamentRelation(int companyId, int pharmacyId, int medicamentId, CancellationToken cancellationToken)
    {
        var pharmacyMedicament = await _databaseContext.PharmacyMedicaments
            .AsNoTracking()
            .Include(x => x.Pharmacy)
            .Include(x => x.Medicament)
            .SingleOrDefaultAsync(x => x.PharmacyId == pharmacyId && x.MedicamentId == medicamentId, cancellationToken);

        return pharmacyMedicament?.Pharmacy.CompanyId != companyId && pharmacyMedicament?.Medicament.CompanyId != companyId
            ? NotFound()
            : null;
    }

    #endregion
}