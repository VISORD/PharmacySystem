using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Models.Order;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IOrderMedicamentRepository
{
    Task<bool> IsExistAsync(
        IDbTransaction transaction,
        int companyId,
        int orderId,
        int medicamentId,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasRequested(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    );

    Task<bool> HasNonApproved(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    );

    Task DeliveryToPharmacies(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    );

    Task<ItemsPagingResult<OrderMedicamentItem>> ListAsync(
        IDbTransaction transaction,
        int orderId,
        OrderMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task<OrderMedicament?> GetAsync(
        IDbTransaction transaction,
        int orderId,
        int medicamentId,
        CancellationToken cancellationToken = default
    );

    Task MergeAsync(
        IDbTransaction transaction,
        OrderMedicament orderMedicament,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        IDbTransaction transaction,
        int orderId,
        int medicamentId,
        CancellationToken cancellationToken = default
    );
}

public sealed class OrderMedicamentRepository : IOrderMedicamentRepository
{
    public async Task<bool> IsExistAsync(
        IDbTransaction transaction,
        int companyId,
        int orderId,
        int medicamentId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [order].[OrderMedicament] om
            JOIN [order].[Order] o ON o.[Id] = om.[OrderId]
            JOIN [pharmacy].[Pharmacy] p ON p.[Id] = o.[PharmacyId]
            WHERE p.[CompanyId] = @CompanyId AND o.[Id] = @OrderId AND om.[MedicamentId] = @MedicamentId;
        ", parameters: new
        {
            CompanyId = companyId,
            OrderId = orderId,
            MedicamentId = medicamentId,
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<bool> HasRequested(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [order].[OrderMedicament]
            WHERE [OrderId] = @OrderId;
        ", parameters: new { OrderId = orderId }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<bool> HasNonApproved(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [order].[OrderMedicament]
            WHERE [OrderId] = @OrderId AND [IsApproved] = 0;
        ", parameters: new { OrderId = orderId }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task DeliveryToPharmacies(
        IDbTransaction transaction,
        int orderId,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            MERGE INTO [pharmacy].[PharmacyMedicament] p
            USING (
                SELECT
                FROM [order].[OrderMedicament] m
                JOIN [order].[Order] o ON o.[Id] = m.[OrderId]
                WHERE m.[OrderId] = @OrderId
            ) o ON p.[PharmacyId] = o.[PharmacyId] AND p.[MedicamentId] = o.[MedicamentId]
            WHEN MATCHED THEN
                UPDATE SET [QuantityOnHand] = p.[QuantityOnHand] + o.[ApprovedCount]
            WHEN NOT MATCHED THEN
                INSERT (
                     [PharmacyId]
                    ,[MedicamentId]
                    ,[QuantityOnHand]
                )
                VALUES (
                     o.[PharmacyId]
                    ,o.[MedicamentId]
                    ,o.[ApprovedCount]
                );
        ", parameters: new { OrderId = orderId }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<ItemsPagingResult<OrderMedicamentItem>> ListAsync(
        IDbTransaction transaction,
        int orderId,
        OrderMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        const string with = @"WITH OrderMedicaments AS (
                SELECT
                     om.[OrderId]
                    ,om.[MedicamentId]
                    ,m.[Name]                         [MedicamentName]
                    ,m.[VendorPrice]
                    ,ISNULL(pm.[QuantityOnHand], 0)   [QuantityOnHand]
                    ,om.[RequestedCount]
                    ,om.[ApprovedCount]
                    ,om.[IsApproved]
                FROM [order].[OrderMedicament] om
                JOIN [medicament].[Medicament] m ON m.[Id] = om.[MedicamentId]
                JOIN [order].[Order] o ON o.[Id] = om.[OrderId]
                JOIN [pharmacy].[Pharmacy] p ON p.[Id] = o.[PharmacyId]
                LEFT JOIN [pharmacy].[PharmacyMedicament] pm ON pm.[PharmacyId] = o.[PharmacyId] AND pm.[MedicamentId] = om.[MedicamentId]
            )";
        var (filters, parameters) = request.SqlFiltering<OrderMedicamentItem>();
        var where = "[OrderId] = @OrderId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("OrderId", "MedicamentId"));

        var @params = new DynamicParameters();
        @params.Add("OrderId", orderId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
            {with}
            SELECT COUNT (*)
            FROM OrderMedicaments
            WHERE {where};

            {with}
            SELECT
                 [OrderId]          [{nameof(OrderMedicamentItem.OrderId)}]
                ,[MedicamentId]     [{nameof(OrderMedicamentItem.MedicamentId)}]
                ,[MedicamentName]   [{nameof(OrderMedicamentItem.MedicamentName)}]
                ,[VendorPrice]      [{nameof(OrderMedicamentItem.VendorPrice)}]
                ,[QuantityOnHand]   [{nameof(OrderMedicamentItem.QuantityOnHand)}]
                ,[RequestedCount]   [{nameof(OrderMedicamentItem.RequestedCount)}]
                ,[ApprovedCount]    [{nameof(OrderMedicamentItem.ApprovedCount)}]
                ,[IsApproved]       [{nameof(OrderMedicamentItem.IsApproved)}]
            FROM OrderMedicaments
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<OrderMedicamentItem>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<OrderMedicamentItem>()
        );
    }

    public async Task<OrderMedicament?> GetAsync(
        IDbTransaction transaction,
        int orderId,
        int medicamentId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<OrderMedicament>(new CommandDefinition($@"
            SELECT
                 [OrderId]          [{nameof(OrderMedicament.OrderId)}]
                ,[MedicamentId]     [{nameof(OrderMedicament.MedicamentId)}]
                ,[RequestedCount]   [{nameof(OrderMedicament.RequestedCount)}]
                ,[ApprovedCount]    [{nameof(OrderMedicament.ApprovedCount)}]
                ,[IsApproved]       [{nameof(OrderMedicament.IsApproved)}]
            FROM [order].[OrderMedicament]
            WHERE [OrderId] = @OrderId AND [MedicamentId] = @MedicamentId;
        ", parameters: new
        {
            OrderId = orderId,
            MedicamentId = medicamentId
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task MergeAsync(
        IDbTransaction transaction,
        OrderMedicament orderMedicament,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            MERGE INTO [order].[OrderMedicament] t
            USING (
                SELECT
                     @{nameof(OrderMedicament.OrderId)}          [OrderId]
                    ,@{nameof(OrderMedicament.MedicamentId)}     [MedicamentId]
                    ,@{nameof(OrderMedicament.RequestedCount)}   [RequestedCount]
                    ,@{nameof(OrderMedicament.ApprovedCount)}    [ApprovedCount]
                    ,@{nameof(OrderMedicament.IsApproved)}       [IsApproved]
            ) s ON s.[OrderId] = t.[OrderId] AND s.[MedicamentId] = t.[MedicamentId]
            WHEN MATCHED THEN
                UPDATE SET
                     [RequestedCount] = s.[RequestedCount]
                    ,[ApprovedCount] = s.[ApprovedCount]
                    ,[IsApproved] = s.[IsApproved]
            WHEN NOT MATCHED THEN
                INSERT (
                     [OrderId]
                    ,[MedicamentId]
                    ,[RequestedCount]
                    ,[ApprovedCount]
                    ,[IsApproved]
                )
                VALUES (
                     s.[OrderId]
                    ,s.[MedicamentId]
                    ,s.[RequestedCount]
                    ,s.[ApprovedCount]
                    ,s.[IsApproved]
                );
        ", parameters: orderMedicament, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(
        IDbTransaction transaction,
        int orderId,
        int medicamentId,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition(@"
            DELETE FROM [order].[OrderMedicament]
            WHERE [OrderId] = @OrderId AND [MedicamentId] = @MedicamentId;
        ", parameters: new
        {
            OrderId = orderId,
            MedicamentId = medicamentId
        }, transaction: transaction, cancellationToken: cancellationToken));
    }
}