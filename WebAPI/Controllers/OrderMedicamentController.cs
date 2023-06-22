using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Order;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/order/{orderId:int}/medicament")]
[Authorize]
public sealed class OrderMedicamentController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public OrderMedicamentController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var query = _databaseContext.OrderMedicaments
            .Include(x => x.Medicament)
            .Where(x => x.OrderId == orderId);

        query = request.Ordering.Aggregate(query, (current, field) => field.IsAscending
            ? current.OrderBy(x => EF.Property<object>(x, field.FieldName))
            : current.OrderByDescending(x => EF.Property<object>(x, field.FieldName)));

        var totalAmount = await query.CountAsync(cancellationToken);
        var orderMedicaments = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsPagingResponse(
            request.Paging,
            request.Ordering,
            totalAmount,
            orderMedicaments.Select(OrderMedicamentListItemModel.From)
        ));
    }

    [HttpGet("{medicamentId:int}")]
    public async Task<IActionResult> Get(
        int orderId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderMedicamentRelation(companyId, orderId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var orderMedicament = await _databaseContext.OrderMedicaments
            .Include(x => x.Order)
            .Include(x => x.Medicament)
            .SingleOrDefaultAsync(x => x.OrderId == orderId && x.MedicamentId == medicamentId, cancellationToken);

        return orderMedicament is not null
            ? Ok(new ItemResponse(OrderMedicamentProfileModel.From(orderMedicament)))
            : NotFound();
    }

    [HttpPut("{medicamentId:int}/request")]
    public async Task<IActionResult> RequestMedicament(
        int orderId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderMedicamentRequestModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderMedicamentRelation(companyId, orderId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var order = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        if (order.Status != OrderStatus.Draft)
        {
            return BadRequest(new ItemResponse(Error: "Non-draft order medicament can't be modified"));
        }

        var orderMedicament = _databaseContext.OrderMedicaments.Update(new OrderMedicament
        {
            OrderId = orderId,
            MedicamentId = medicamentId,
            RequestedCount = model.Count!.Value
        });

        var hasManyItems = orderMedicament.Entity.RequestedCount > 1;
        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = orderId,
            Event = $"{orderMedicament.Entity.RequestedCount} item{(hasManyItems ? "s" : "")} of \"{orderMedicament.Entity.Medicament.Name}\" medicament {(hasManyItems ? "were" : "was")} requested",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{medicamentId:int}/approve")]
    public async Task<IActionResult> ApproveMedicament(
        int orderId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderMedicamentRequestModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderMedicamentRelation(companyId, orderId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var orderMedicament = await _databaseContext.OrderMedicaments
            .Include(x => x.Order)
            .SingleAsync(x => x.OrderId == orderId && x.MedicamentId == medicamentId, cancellationToken);

        if (orderMedicament.Order.Status != OrderStatus.Ordered)
        {
            return BadRequest(new ItemResponse(Error: "Non-processing order medicament can't be modified"));
        }

        orderMedicament.ApprovedCount = model.Count!.Value;
        orderMedicament.IsApproved = true;
        _databaseContext.OrderMedicaments.Update(orderMedicament);

        var hasManyItems = orderMedicament.ApprovedCount > 1;
        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = orderId,
            Event = $"{orderMedicament.ApprovedCount} item{(hasManyItems ? "s" : "")} of \"{orderMedicament.Medicament.Name}\" medicament {(hasManyItems ? "were" : "was")} approved",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{medicamentId:int}/disapprove")]
    public async Task<IActionResult> DisapproveMedicament(
        int orderId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderMedicamentRelation(companyId, orderId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var orderMedicament = await _databaseContext.OrderMedicaments
            .Include(x => x.Order)
            .SingleAsync(x => x.OrderId == orderId && x.MedicamentId == medicamentId, cancellationToken);

        if (orderMedicament.Order.Status != OrderStatus.Ordered)
        {
            return BadRequest(new ItemResponse(Error: "Non-processing order medicament can't be modified"));
        }

        orderMedicament.ApprovedCount = null;
        orderMedicament.IsApproved = false;
        _databaseContext.OrderMedicaments.Update(orderMedicament);

        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = orderId,
            Event = $"\"{orderMedicament.Medicament.Name}\" medicament was disapproved",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyOrderRelation(int companyId, int orderId, CancellationToken cancellationToken)
    {
        var pharmacyMedicament = await _databaseContext.Orders
            .AsNoTracking()
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        return pharmacyMedicament?.Pharmacy.CompanyId != companyId
            ? NotFound()
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyOrderMedicamentRelation(int companyId, int orderId, int medicamentId, CancellationToken cancellationToken)
    {
        var orderMedicament = await _databaseContext.OrderMedicaments
            .AsNoTracking()
            .Include(x => x.Order)
            .ThenInclude(x => x.Pharmacy)
            .Include(x => x.Medicament)
            .SingleOrDefaultAsync(x => x.OrderId == orderId && x.MedicamentId == medicamentId, cancellationToken);

        return orderMedicament?.Order.Pharmacy.CompanyId != companyId && orderMedicament?.Medicament.CompanyId != companyId
            ? NotFound()
            : null;
    }

    #endregion
}