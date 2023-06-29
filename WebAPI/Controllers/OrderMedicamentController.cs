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
[Route("api/order/{orderId:int}/medicament")]
[Authorize]
public sealed class OrderMedicamentController : ControllerBase
{
    private readonly IOrderMedicamentRepository _orderMedicamentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderHistoryRepository _orderHistoryRepository;
    private readonly IMedicamentRepository _medicamentRepository;

    public OrderMedicamentController(
        IOrderMedicamentRepository orderMedicamentRepository,
        IOrderRepository orderRepository,
        IOrderHistoryRepository orderHistoryRepository,
        IMedicamentRepository medicamentRepository
    )
    {
        _orderMedicamentRepository = orderMedicamentRepository;
        _orderRepository = orderRepository;
        _orderHistoryRepository = orderHistoryRepository;
        _medicamentRepository = medicamentRepository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List(
        [Database(isReadOnly: true)] SqlConnection connection,
        int orderId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderRelation(transaction, companyId, orderId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var result = await _orderMedicamentRepository.ListAsync(transaction, orderId, request, cancellationToken);
        return Ok(new ItemsPagingResponse(
            result.TotalAmount,
            result.Items.Select(OrderMedicamentItemPagingModel.From)
        ));
    }

    [HttpPut("{medicamentId:int}/request")]
    public async Task<IActionResult> RequestMedicament(
        [Database] SqlConnection connection,
        int orderId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderMedicamentRequestModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderMedicamentRelation(transaction, companyId, orderId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var medicament = await _medicamentRepository.GetAsync(transaction, medicamentId, cancellationToken);
        if (medicament is null)
        {
            return NotFound();
        }

        var order = await _orderRepository.GetAsync(transaction, orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Draft)
        {
            return BadRequest(new ItemResponse(Error: "Non-draft order medicament can't be modified"));
        }

        if (model.Count > 0)
        {
            await _orderMedicamentRepository.MergeAsync(transaction, new OrderMedicament
            {
                OrderId = orderId,
                MedicamentId = medicamentId,
                RequestedCount = model.Count.Value
            }, cancellationToken);

            var hasManyItems = model.Count.Value > 1;
            await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
            {
                OrderId = orderId,
                Event = $"{model.Count.Value} item{(hasManyItems ? "s" : "")} of \"{medicament.Name}\" medicament {(hasManyItems ? "were" : "was")} requested",
                Timestamp = DateTimeOffset.Now,
            }, cancellationToken);
        }
        else
        {
            await _orderMedicamentRepository.DeleteAsync(transaction, orderId, medicamentId, cancellationToken);

            await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
            {
                OrderId = orderId,
                Event = $"\"{medicament.Name}\" medicament request was canceled",
                Timestamp = DateTimeOffset.Now,
            }, cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{medicamentId:int}/approve")]
    public async Task<IActionResult> ApproveMedicament(
        [Database] SqlConnection connection,
        int orderId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        [FromBody] OrderMedicamentRequestModel model,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderMedicamentRelation(transaction, companyId, orderId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var medicament = await _medicamentRepository.GetAsync(transaction, medicamentId, cancellationToken);
        if (medicament is null)
        {
            return NotFound();
        }

        var order = await _orderRepository.GetAsync(transaction, orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        var orderMedicament = await _orderMedicamentRepository.GetAsync(transaction, orderId, medicamentId, cancellationToken);
        if (orderMedicament is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Ordered)
        {
            return BadRequest(new ItemResponse(Error: "Non-processing order medicament can't be modified"));
        }

        orderMedicament.ApprovedCount = model.Count!.Value;
        orderMedicament.IsApproved = true;

        await _orderMedicamentRepository.MergeAsync(transaction, orderMedicament, cancellationToken);

        var hasManyItems = model.Count!.Value > 1;
        await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
        {
            OrderId = orderId,
            Event = $"{model.Count!.Value} item{(hasManyItems ? "s" : "")} of \"{medicament.Name}\" medicament {(hasManyItems ? "were" : "was")} approved",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{medicamentId:int}/disapprove")]
    public async Task<IActionResult> DisapproveMedicament(
        [Database] SqlConnection connection,
        int orderId,
        int medicamentId,
        [FromClaim(ClaimTypes.CompanyId)] int companyId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);

        var validationResult = await ValidateCompanyOrderMedicamentRelation(transaction, companyId, orderId, medicamentId, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var medicament = await _medicamentRepository.GetAsync(transaction, medicamentId, cancellationToken);
        if (medicament is null)
        {
            return NotFound();
        }

        var order = await _orderRepository.GetAsync(transaction, orderId, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        var orderMedicament = await _orderMedicamentRepository.GetAsync(transaction, orderId, medicamentId, cancellationToken);
        if (orderMedicament is null)
        {
            return NotFound();
        }

        if (order.Status != OrderStatus.Ordered)
        {
            return BadRequest(new ItemResponse(Error: "Non-processing order medicament can't be modified"));
        }

        orderMedicament.ApprovedCount = null;
        orderMedicament.IsApproved = false;

        await _orderMedicamentRepository.MergeAsync(transaction, orderMedicament, cancellationToken);

        await _orderHistoryRepository.AddAsync(transaction, new OrderHistory
        {
            OrderId = orderId,
            Event = $"\"{medicament.Name}\" medicament was disapproved",
            Timestamp = DateTimeOffset.Now,
        }, cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return NoContent();
    }

    #region Validation

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyOrderRelation(IDbTransaction transaction, int companyId, int orderId, CancellationToken cancellationToken)
    {
        return !await _orderRepository.IsExistAsync(transaction, companyId, orderId, cancellationToken)
            ? NotFound()
            : null;
    }

    [NonAction]
    private async Task<IActionResult?> ValidateCompanyOrderMedicamentRelation(IDbTransaction transaction, int companyId, int orderId, int medicamentId, CancellationToken cancellationToken)
    {
        return !await _orderMedicamentRepository.IsExistAsync(transaction, companyId, orderId, medicamentId, cancellationToken)
            ? NotFound()
            : null;
    }

    #endregion
}