using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Entities.Order;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IOrderHistoryRepository
{
    Task<IEnumerable<OrderHistory>> ListAsync(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        IDbTransaction transaction,
        OrderHistory orderHistoryRecord,
        CancellationToken cancellationToken = default
    );
}

public sealed class OrderHistoryRepository : IOrderHistoryRepository
{
    public async Task<IEnumerable<OrderHistory>> ListAsync(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QueryAsync<OrderHistory>(new CommandDefinition($@"
            SELECT
                 [Id]          [{nameof(OrderHistory.Id)}]
                ,[OrderId]     [{nameof(OrderHistory.OrderId)}]
                ,[Timestamp]   [{nameof(OrderHistory.Timestamp)}]
                ,[Event]       [{nameof(OrderHistory.Event)}]
            FROM [order].[OrderHistory]
            WHERE [OrderId] = @OrderId;
        ", parameters: new { OrderId = orderId }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        IDbTransaction transaction,
        OrderHistory orderHistoryRecord,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            INSERT INTO [order].[OrderHistory] (
                 [OrderId]
                ,[Timestamp]
                ,[Event]
            )
            VALUES (
                 @{nameof(OrderHistory.OrderId)}
                ,@{nameof(OrderHistory.Timestamp)}
                ,@{nameof(OrderHistory.Event)}
            );
        ", parameters: orderHistoryRecord, transaction: transaction, cancellationToken: cancellationToken));
    }
}