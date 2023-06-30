using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PharmacySystem.WebAPI.Authentication.Claims;
using PharmacySystem.WebAPI.Database.Connection;
using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Database.Repositories;
using PharmacySystem.WebAPI.Models.Common;
using PharmacySystem.WebAPI.Models.Order;

namespace PharmacySystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IPharmacyRepository _pharmacyRepository;
    private readonly IOrderMedicamentRepository _orderMedicamentRepository;

    public OrderController(
        IOrderRepository orderRepository,
        IOrderHistoryRepository orderHistoryRepository,
        IPharmacyRepository pharmacyRepository,
        IOrderMedicamentRepository orderMedicamentRepository
    )
    {
        _orderRepository = orderRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _pharmacyRepository = pharmacyRepository;
        _orderMedicamentRepository = orderMedicamentRepository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [Database(isReadOnly: true)] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var result = await _orderRepository.ListAsync(transaction, companyId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(OrderItemPagingModel.From)
        ));
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [Database] SqlConnection connection,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderCreationModel model,
        CancellationToken cancellationToken
    )
    {
        var order = model.To();

        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyPharmacyRelation(transaction, companyId, order.PharmacyId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var orderId = await _orderRepository.AddAsync(transaction, order, cancellationToken);
        await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was created",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> Get(
        [Database(isReadOnly: true)] SqlConnection connection,
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(transaction, companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var order = await _orderRepository.GetProfileAsync(transaction, orderId, cancellationToken);
        return order is not null
            ? Ok(new ItemResponse(OrderProfileModel.From(order)))
            : NotFound();
    }

    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> Delete(
        [Database] SqlConnection connection,
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(transaction, companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var order = await _orderRepository.GetAsync(transaction, orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Draft)
        {
            return BadRequest(new ItemResponse(Error: "Non-draft order can't be deleted"));
        }

        await _orderRepository.DeleteAsync(transaction, orderId, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{orderId:int}/launch")]
    public async Task<IActionResult> Launch(
        [Database] SqlConnection connection,
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(transaction, companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var order = await _orderRepository.GetAsync(transaction, orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Draft)
        {
            return BadRequest(new ItemResponse(Error: "Non-draft order can't be launched"));
        }

        if (!await _orderMedicamentRepository.HasRequested(transaction, orderId, cancellationToken))
        {
            return BadRequest(new ItemResponse(Error: "No requested medicaments"));
        }

        order.Status = OrderStatus.Ordered;
        order.OrderedAt = DateTimeOffset.Now;
        order.UpdatedAt = DateTimeOffset.Now;
        await _orderRepository.UpdateAsync(transaction, order, cancellationToken);

        await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was launched",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{orderId:int}/ship")]
    public async Task<IActionResult> Ship(
        [Database] SqlConnection connection,
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(transaction, companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var order = await _orderRepository.GetAsync(transaction, orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Ordered)
        {
            return BadRequest(new ItemResponse(Error: "Non-processed order can't be shipped"));
        }

        if (await _orderMedicamentRepository.HasNonApproved(transaction, orderId, cancellationToken))
        {
            return BadRequest(new ItemResponse(Error: "All order medicaments should be approved before"));
        }

        order.Status = OrderStatus.Shipped;
        order.UpdatedAt = DateTimeOffset.Now;
        await _orderRepository.UpdateAsync(transaction, order, cancellationToken);

        await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was shipped",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{orderId:int}/complete")]
    public async Task<IActionResult> Complete(
        [Database] SqlConnection connection,
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(transaction, companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var order = await _orderRepository.GetAsync(transaction, orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Shipped)
        {
            return BadRequest(new ItemResponse(Error: "Non-shipped order can't be completed"));
        }

        await _orderMedicamentRepository.DeliveryToPharmacies(transaction, orderId, cancellationToken);

        order.Status = OrderStatus.Delivered;
        order.UpdatedAt = DateTimeOffset.Now;
        await _orderRepository.UpdateAsync(transaction, order, cancellationToken);

        await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
        {
            OrderId = orderId,
            Event = "Order was completed",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("{orderId:int}/history")]
    public async Task<IActionResult> History(
        [Database(isReadOnly: true)] SqlConnection connection,
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(transaction, companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var historyRecords = await _orderHistoryRepository.ListAsync(transaction, orderId, cancellationToken);
        return Ok(new ItemsResponse(historyRecords.Select(OrderHistoryRecordModel.From)));
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
    private async Task<IActionResult?> ValidateCompanyOrderRelation(IDbTransaction transaction, int companyId, int orderId, CancellationToken cancellationToken)
    {
        return !await _orderRepository.IsExistAsync(transaction, companyId, orderId, cancellationToken)
            ? NotFound()
            : null;
    }

    #endregion
}