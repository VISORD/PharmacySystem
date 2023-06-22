using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database;
using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Order;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class OrderController : ControllerBase
{
    private readonly DatabaseContext _databaseContext;

    public OrderController(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var query = _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .Where(x => x.Pharmacy.CompanyId == companyId);

        query = request.Ordering.Aggregate(query, (current, field) => field.IsAscending
            ? current.OrderBy(x => EF.Property<object>(x, field.FieldName))
            : current.OrderByDescending(x => EF.Property<object>(x, field.FieldName)));

        var totalAmount = await query.CountAsync(cancellationToken);
        var orders = await query
            .Skip(request.Paging.Offset!.Value)
            .Take(request.Paging.Size!.Value)
            .ToArrayAsync(cancellationToken);

        var orderIds = orders.Select(x => x.Id).ToHashSet();
        var medicamentItemsCounts = await _databaseContext.OrderMedicaments
            .Where(x => orderIds.Contains(x.OrderId))
            .GroupBy(x => x.OrderId)
            .Select(x => new { OrderId = x.Key, MedicamentItemsCount = x.Count() })
            .ToDictionaryAsync(x => x.OrderId, x => x.MedicamentItemsCount, cancellationToken);

        return Ok(new ItemsPagingResponse(
            request.Paging,
            request.Ordering,
            totalAmount,
            orders.Select(x => OrderItemPagingModel.From(x, medicamentItemsCounts))
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderCreationModel model,
        CancellationToken cancellationToken
    )
    {
        var order = model.To();

        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(companyId, order.PharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _databaseContext.Orders.AddAsync(order, cancellationToken);

        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = result.Entity.Id,
            Event = "Order was created",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Ok(new ItemResponse(result.Entity.Id));
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> Get(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var order = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        return order is not null
            ? Ok(new ItemResponse(OrderProfileModel.From(order)))
            : NotFound();
    }

    [HttpPut("{orderId:int}")]
    public async Task<IActionResult> Update(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderUpdateModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var existing = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        if (existing is null)
        {
            return NotFound();
        }

        if (existing.Status != OrderStatus.Draft)
        {
            return BadRequest(new ItemResponse(Error: "Non-draft order can't be changed"));
        }

        var order = model.To(orderId, existing.PharmacyId);
        _databaseContext.Orders.Update(order);

        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was updated",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> Delete(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var order = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        _databaseContext.Orders.Remove(order);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{orderId:int}/launch")]
    public async Task<IActionResult> Launch(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var order = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Draft)
        {
            return BadRequest(new ItemResponse(Error: "Non-draft order can't be launched"));
        }

        order.Status = OrderStatus.Ordered;
        _databaseContext.Orders.Update(order);

        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was launched",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{orderId:int}/ship")]
    public async Task<IActionResult> Ship(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var order = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Ordered)
        {
            return BadRequest(new ItemResponse(Error: "Non-processed order can't be shipped"));
        }

        var hasNonProcessedMedicaments = await _databaseContext.OrderMedicaments
            .AnyAsync(x => x.OrderId == orderId && !x.IsApproved, cancellationToken);
        if (hasNonProcessedMedicaments)
        {
            return BadRequest(new ItemResponse(Error: "All order medicaments should be approved before"));
        }

        order.Status = OrderStatus.Shipped;
        _databaseContext.Orders.Update(order);

        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was shipped",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{orderId:int}/complete")]
    public async Task<IActionResult> Complete(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var order = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Shipped)
        {
            return BadRequest(new ItemResponse(Error: "Non-shipped order can't be completed"));
        }

        order.Status = OrderStatus.Delivered;
        _databaseContext.Orders.Update(order);

        var orderMedicaments = _databaseContext.OrderMedicaments
            .Where(x => x.OrderId == orderId)
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken);

        await foreach (var orderMedicament in orderMedicaments)
        {
            var pharmacyMedicament = await _databaseContext.PharmacyMedicaments
                .SingleOrDefaultAsync(x => x.PharmacyId == order.PharmacyId && x.MedicamentId == orderMedicament.MedicamentId, cancellationToken);

            if (pharmacyMedicament is null)
            {
                pharmacyMedicament = new PharmacyMedicament
                {
                    PharmacyId = order.PharmacyId,
                    MedicamentId = orderMedicament.MedicamentId,
                };
            }

            pharmacyMedicament.QuantityOnHand += orderMedicament.ApprovedCount!.Value;
            _databaseContext.PharmacyMedicaments.Update(pharmacyMedicament);
        }

        await _databaseContext.OrderHistory.AddAsync(new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was completed",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await _databaseContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{orderId:int}/history")]
    public async Task<IActionResult> History(
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await _databaseContext.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var order = await _databaseContext.Orders
            .Include(x => x.Pharmacy)
            .SingleOrDefaultAsync(x => x.Id == orderId && x.Pharmacy.CompanyId == companyId, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        var historyRecords = await _databaseContext.OrderHistory
            .Where(x => x.OrderId == orderId)
            .ToArrayAsync(cancellationToken);

        return Ok(new ItemsResponse(historyRecords.Select(OrderHistoryRecordModel.From)));
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

    #endregion
}