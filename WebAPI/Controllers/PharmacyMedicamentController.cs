using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database.Connection;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Database.Repositories;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/pharmacy/{pharmacyId:int}/medicament")]
[Authorize]
public sealed class PharmacyMedicamentController : ControllerBase
{
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IPharmacyMedicamentRepository _pharmacyMedicamentRepository;
    private readonly IPharmacyMedicamentRateRepository _pharmacyMedicamentRateRepository;
    private readonly IPharmacyMedicamentSaleRepository _pharmacyMedicamentSaleRepository;
    private readonly IPharmacyMedicamentOrderRepository _pharmacyMedicamentOrderRepository;
    private readonly IMedicamentRepository _medicamentRepository;

    public PharmacyMedicamentController(
        IPharmacyRepository pharmacyRepository,
        IPharmacyMedicamentRepository pharmacyMedicamentRepository,
        IPharmacyMedicamentRateRepository pharmacyMedicamentRateRepository,
        IPharmacyMedicamentSaleRepository pharmacyMedicamentSaleRepository,
        IPharmacyMedicamentOrderRepository pharmacyMedicamentOrderRepository,
        IMedicamentRepository medicamentRepository
    )
    {
        _pharmacyRepository = pharmacyRepository;
        _pharmacyMedicamentRepository = pharmacyMedicamentRepository;
        _pharmacyMedicamentRateRepository = pharmacyMedicamentRateRepository;
        _pharmacyMedicamentSaleRepository = pharmacyMedicamentSaleRepository;
        _pharmacyMedicamentOrderRepository = pharmacyMedicamentOrderRepository;
        _medicamentRepository = medicamentRepository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [Database(isReadOnly: true)] SqlConnection connection,
        int pharmacyId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(transaction, companyId, pharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _pharmacyMedicamentRepository.ListAsync(transaction, pharmacyId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(PharmacyMedicamentItemPagingModel.From)
        ));
    }

    [HttpGet("{medicamentId:int}")]
    public async Task<IActionResult> Get(
        [Database(isReadOnly: true)] SqlConnection connection,
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(transaction, companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var pharmacyMedicament = await _pharmacyMedicamentRepository.GetProfileAsync(transaction, pharmacyId, medicamentId, cancellationToken: cancellationToken);
        return pharmacyMedicament is not null
            ? Ok(new ItemResponse(PharmacyMedicamentProfileModel.From(pharmacyMedicament)))
            : NotFound();
    }

    [HttpPost("{medicamentId:int}/rate/list")]
    public async Task<IActionResult> Rates(
        [Database(isReadOnly: true)] SqlConnection connection,
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentRateItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(transaction, companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _pharmacyMedicamentRateRepository.ListAsync(transaction, pharmacyId, medicamentId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(PharmacyMedicamentRateItemPagingModel.From)
        ));
    }

    [HttpPut("{medicamentId:int}/rate")]
    public async Task<IActionResult> Rate(
        [Database] SqlConnection connection,
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentRateModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(transaction, companyId, pharmacyId, medicamentId, cancellationToken);
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

        var pharmacyMedicament = await _pharmacyMedicamentRepository.GetProfileAsync(transaction, pharmacyId, medicamentId, cancellationToken: cancellationToken);
        if (pharmacyMedicament is null)
        {
            await _pharmacyMedicamentRepository.AddAsync(transaction, pharmacyId, medicamentId, cancellationToken);
        }

        var intersectedRates = (await _pharmacyMedicamentRateRepository.GetIntersectedAsync(transaction, rate, cancellationToken)).ToArray();

        var ratesToAdd = new List<PharmacyMedicamentRate>();
        var ratesToDelete = new List<PharmacyMedicamentRate>();

        if (intersectedRates.Length == 0)
        {
            ratesToAdd.Add(rate);
        }
        else
        {
            PharmacyMedicamentRate? left = null;
            PharmacyMedicamentRate? right = null;
            foreach (var intersectedRate in intersectedRates)
            {
                if (rate.StartDate <= intersectedRate.StartDate && intersectedRate.StopDate <= rate.StopDate)
                {
                    ratesToDelete.Add(intersectedRate);
                    continue;
                }

                if (intersectedRate.StartDate < rate.StartDate && rate.StartDate < intersectedRate.StopDate)
                {
                    left = intersectedRate;
                }

                if (intersectedRate.StartDate < rate.StopDate && rate.StopDate < intersectedRate.StopDate)
                {
                    right = intersectedRate;
                }
            }

            var flag = left is null && right is null;

            if (left is not null && right is null)
            {
                if (left.RetailPrice == rate.RetailPrice)
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = left.StartDate,
                        StopDate = rate.StopDate,
                        RetailPrice = rate.RetailPrice
                    });
                }
                else
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = left.StartDate,
                        StopDate = rate.StartDate.AddDays(-1),
                        RetailPrice = left.RetailPrice
                    });

                    flag = true;
                }

                ratesToDelete.Add(left);
            }

            if (right is not null && left is null)
            {
                if (right.RetailPrice == rate.RetailPrice)
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = rate.StartDate,
                        StopDate = right.StopDate,
                        RetailPrice = rate.RetailPrice
                    });
                }
                else
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = rate.StopDate.AddDays(1),
                        StopDate = right.StopDate,
                        RetailPrice = right.RetailPrice
                    });

                    flag = true;
                }

                ratesToDelete.Add(right);
            }

            if (left is not null && right is not null)
            {
                if (left.RetailPrice == rate.RetailPrice && right.RetailPrice == rate.RetailPrice)
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = left.StartDate,
                        StopDate = right.StopDate,
                        RetailPrice = rate.RetailPrice
                    });
                }
                else if (left.RetailPrice == rate.RetailPrice)
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = left.StartDate,
                        StopDate = rate.StopDate,
                        RetailPrice = rate.RetailPrice
                    });

                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = rate.StopDate.AddDays(1),
                        StopDate = right.StopDate,
                        RetailPrice = right.RetailPrice
                    });
                } 
                else if (right.RetailPrice == rate.RetailPrice)
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = left.StartDate,
                        StopDate = rate.StartDate.AddDays(-1),
                        RetailPrice = rate.RetailPrice
                    });

                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = rate.StartDate,
                        StopDate = right.StopDate,
                        RetailPrice = rate.RetailPrice
                    });
                }
                else
                {
                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = left.StartDate,
                        StopDate = rate.StartDate.AddDays(-1),
                        RetailPrice = right.RetailPrice
                    });

                    ratesToAdd.Add(new PharmacyMedicamentRate
                    {
                        PharmacyId = pharmacyId,
                        MedicamentId = medicamentId,
                        StartDate = rate.StopDate.AddDays(1),
                        StopDate = right.StopDate,
                        RetailPrice = right.RetailPrice
                    });

                    flag = true;
                }

                ratesToDelete.Add(left);
                ratesToDelete.Add(right);
            }

            if (flag)
            {
                ratesToAdd.Add(rate);
            }
        }

        if (ratesToDelete.Count > 0)
        {
            await _pharmacyMedicamentRateRepository.DeleteAsync(transaction, ratesToDelete, cancellationToken);
        }

        if (ratesToAdd.Count > 0)
        {
            await _pharmacyMedicamentRateRepository.AddAsync(transaction, ratesToAdd, cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{medicamentId:int}/sale/list")]
    public async Task<IActionResult> Sales(
        [Database(isReadOnly: true)] SqlConnection connection,
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentSaleItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(transaction, companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _pharmacyMedicamentSaleRepository.ListAsync(transaction, pharmacyId, medicamentId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(PharmacyMedicamentSaleItemPagingModel.From)
        ));
    }

    [HttpPut("{medicamentId:int}/sale")]
    public async Task<IActionResult> Sale(
        [Database] SqlConnection connection,
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentSaleModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(transaction, companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var pharmacyMedicament = await _pharmacyMedicamentRepository.GetAsync(transaction, pharmacyId, medicamentId, cancellationToken: cancellationToken);
        if (pharmacyMedicament is null)
        {
            await _pharmacyMedicamentRepository.AddAsync(transaction, pharmacyId, medicamentId, cancellationToken);
        }

        if (pharmacyMedicament is not { QuantityOnHand: > 0 })
        {
            return BadRequest(new ItemResponse(Error: "Out of stock"));
        }

        if (pharmacyMedicament.QuantityOnHand - model.UnitsSold < 0)
        {
            return BadRequest(new ItemResponse(Error: "Not enough units to sale"));
        }

        var salePrice = await _pharmacyMedicamentRateRepository.GetAsync(transaction, pharmacyId, medicamentId, model.SoldAt.LocalDateTime, cancellationToken);
        if (salePrice is null)
        {
            return BadRequest(new ItemResponse(Error: "No retail price for specified date"));
        }

        await _pharmacyMedicamentSaleRepository.AddAsync(transaction, new PharmacyMedicamentSale
        {
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId,
            SoldAt = model.SoldAt,
            SalePrice = salePrice.RetailPrice,
            UnitsSold = model.UnitsSold!.Value,
        }, cancellationToken);

        pharmacyMedicament.QuantityOnHand -= model.UnitsSold!.Value;
        await _pharmacyMedicamentRepository.UpdateAsync(transaction, pharmacyMedicament, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{medicamentId:int}/order/list")]
    public async Task<IActionResult> Orders(
        [Database(isReadOnly: true)] SqlConnection connection,
        int pharmacyId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] PharmacyMedicamentOrderItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyMedicamentRelation(transaction, companyId, pharmacyId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _pharmacyMedicamentOrderRepository.ListAsync(transaction, pharmacyId, medicamentId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(PharmacyMedicamentOrderItemPagingModel.From)
        ));
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyPharmacyRelation(IDbTransaction transaction, int companyId, int pharmacyId, CancellationToken cancellationToken)
    {
        return !await _pharmacyRepository.IsExistAsync(transaction, companyId, pharmacyId, cancellationToken)
            ? NotFound()
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyPharmacyMedicamentRelation(IDbTransaction transaction, int companyId, int pharmacyId, int medicamentId, CancellationToken cancellationToken)
    {
        return !await _pharmacyRepository.IsExistAsync(transaction, companyId, pharmacyId, cancellationToken) ||
               !await _medicamentRepository.IsExistAsync(transaction, companyId, medicamentId, cancellationToken)
            ? NotFound()
            : null;
    }

    #endregion
}