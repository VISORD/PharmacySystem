using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Models.Order;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IOrderRepository
{
    Task<bool> IsExistAsync(IDbTransaction transaction,
        int companyId,
        int id,
        CancellationToken cancellationToken = default
    );

    Task<ItemsPagingResult<OrderItem>> ListAsync(
        IDbTransaction transaction,
        int companyId,
        OrderItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task<int> AddAsync(
        IDbTransaction transaction,
        Order order,
        CancellationToken cancellationToken = default
    );

    Task<Order?> GetAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );

    Task<OrderProfile?> GetProfileAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        IDbTransaction transaction,
        Order order,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );
}

public sealed class OrderRepository : IOrderRepository
{
    public async Task<bool> IsExistAsync(
        IDbTransaction transaction,
        int companyId,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [order].[Order] o
            JOIN [pharmacy].[Pharmacy] p ON p.[Id] = o.[PharmacyId]
            WHERE p.[CompanyId] = @CompanyId AND o.[Id] = @Id;
        ", parameters: new
        {
            CompanyId = companyId,
            Id = id
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<ItemsPagingResult<OrderItem>> ListAsync(
        IDbTransaction transaction,
        int companyId,
        OrderItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        const string with = @"WITH Orders AS (
                SELECT
                     o.[Id]
                    ,o.[PharmacyId]
                    ,p.[Name]                   [PharmacyName]
                    ,p.[Address]                [PharmacyAddress]
                    ,p.[CompanyId]
                    ,o.[StatusId]
                    ,o.[OrderedAt]
                    ,o.[UpdatedAt]
                    ,COUNT(om.[MedicamentId])   [MedicamentItemCount]
                FROM [order].[Order] o
                JOIN [pharmacy].[Pharmacy] p ON p.[Id] = o.[PharmacyId]
                LEFT JOIN [order].[OrderMedicament] om ON om.[OrderId] = o.[Id]
                GROUP BY
                     o.[Id]
                    ,o.[PharmacyId]
                    ,p.[Name]
                    ,p.[Address]
                    ,p.[CompanyId]
                    ,o.[StatusId]
                    ,o.[OrderedAt]
                    ,o.[UpdatedAt]
            )";
        var (filters, parameters) = request.SqlFiltering<OrderItem>();
        var where = "[CompanyId] = @CompanyId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("Id"));

        var @params = new DynamicParameters();
        @params.Add("CompanyId", companyId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
            {with}
            SELECT COUNT (*)
            FROM Orders
            WHERE {where};

            {with}
            SELECT
                 [Id]                    [{nameof(OrderItem.Id)}]
                ,[PharmacyId]            [{nameof(OrderItem.PharmacyId)}]
                ,[PharmacyName]          [{nameof(OrderItem.PharmacyName)}]
                ,[PharmacyAddress]       [{nameof(OrderItem.PharmacyAddress)}]
                ,[MedicamentItemCount]   [{nameof(OrderItem.MedicamentItemCount)}]
                ,[StatusId]              [{nameof(OrderItem.Status)}]
                ,[OrderedAt]             [{nameof(OrderItem.OrderedAt)}]
                ,[UpdatedAt]             [{nameof(OrderItem.UpdatedAt)}]
            FROM Orders
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<OrderItem>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<OrderItem>()
        );
    }

    public async Task<int> AddAsync(
        IDbTransaction transaction,
        Order order,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleAsync<int>(new CommandDefinition($@"
            DECLARE @Id [core].[IntegerItem];

            INSERT INTO [order].[Order] (
                 [PharmacyId]
                ,[StatusId]
                ,[OrderedAt]
                ,[UpdatedAt]
            )
            OUTPUT INSERTED.[Id] INTO @Id
            VALUES (
                 @{nameof(Order.PharmacyId)}
                ,@{nameof(Order.Status)}
                ,@{nameof(Order.OrderedAt)}
                ,@{nameof(Order.UpdatedAt)}
            );

            SELECT [Value] FROM @Id;
        ", parameters: order, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<Order?> GetAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<Order>(new CommandDefinition($@"
            SELECT
                 o.[Id]           [{nameof(Order.Id)}]
                ,o.[PharmacyId]   [{nameof(Order.PharmacyId)}]
                ,o.[StatusId]     [{nameof(Order.Status)}]
                ,o.[OrderedAt]    [{nameof(Order.OrderedAt)}]
                ,o.[UpdatedAt]    [{nameof(Order.UpdatedAt)}]
            FROM [order].[Order] o
            JOIN [pharmacy].[Pharmacy] p ON p.[Id] = o.[PharmacyId]
            WHERE o.[Id] = @Id;
        ", parameters: new { Id = id }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<OrderProfile?> GetProfileAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<OrderProfile>(new CommandDefinition($@"
            SELECT
                 o.[Id]           [{nameof(OrderProfile.Id)}]
                ,o.[PharmacyId]   [{nameof(OrderProfile.PharmacyId)}]
                ,p.[Name]         [{nameof(OrderProfile.PharmacyName)}]
                ,p.[Address]      [{nameof(OrderProfile.PharmacyAddress)}]
                ,o.[StatusId]     [{nameof(OrderProfile.Status)}]
                ,o.[OrderedAt]    [{nameof(OrderProfile.OrderedAt)}]
                ,o.[UpdatedAt]    [{nameof(OrderProfile.UpdatedAt)}]
            FROM [order].[Order] o
            JOIN [pharmacy].[Pharmacy] p ON p.[Id] = o.[PharmacyId]
            WHERE o.[Id] = @Id;
        ", parameters: new { Id = id }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        IDbTransaction transaction,
        Order order,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            UPDATE [order].[Order]
            SET
                 [Status]    = @{nameof(Order.Status)}
                ,[OrderedAt] = @{nameof(Order.OrderedAt)}
                ,[UpdatedAt] = @{nameof(Order.UpdatedAt)}
            FROM [order].[Order]
            WHERE [Id] = @{nameof(Order.Id)};
        ", parameters: order, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            DELETE FROM [order].[Order]
            WHERE [Id] = @Id;
        ", parameters: new { Id = id }, transaction: transaction, cancellationToken: cancellationToken));
    }
}