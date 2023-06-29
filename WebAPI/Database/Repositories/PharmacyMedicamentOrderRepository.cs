using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IPharmacyMedicamentOrderRepository
{
    Task<ItemsPagingResult<PharmacyMedicamentOrderItem>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        PharmacyMedicamentOrderItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );
}

public sealed class PharmacyMedicamentOrderRepository : IPharmacyMedicamentOrderRepository
{
    public async Task<ItemsPagingResult<PharmacyMedicamentOrderItem>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        PharmacyMedicamentOrderItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        const string with = @"WITH PharmacyMedicamentOrders AS (
                SELECT
                     pm.[PharmacyId]
                    ,pm.[MedicamentId]
                    ,om.[OrderId]
                    ,ISNULL(om.[ApprovedCount], om.[RequestedCount]) [OrderCount]
                    ,o.[StatusId]
                    ,o.[OrderedAt]
                    ,o.[UpdatedAt]
                FROM [pharmacy].[PharmacyMedicament] pm
                JOIN [order].[Order] o ON o.[PharmacyId] = pm.[PharmacyId]
                JOIN [order].[OrderMedicament] om ON om.[OrderId] = o.[Id] AND om.[MedicamentId] = pm.[MedicamentId]
            )";
        var (filters, parameters) = request.SqlFiltering<PharmacyMedicamentOrderItem>();
        var where = "[PharmacyId] = @PharmacyId AND [MedicamentId] = @MedicamentId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("PharmacyId", "MedicamentId", "OrderId"));

        var @params = new DynamicParameters();
        @params.Add("PharmacyId", pharmacyId);
        @params.Add("MedicamentId", medicamentId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
            {with}
            SELECT COUNT (*)
            FROM PharmacyMedicamentOrders
            WHERE {where};

            {with}
            SELECT
                 [PharmacyId]       [{nameof(PharmacyMedicamentOrderItem.PharmacyId)}]
                ,[MedicamentId]     [{nameof(PharmacyMedicamentOrderItem.MedicamentId)}]
                ,[OrderId]          [{nameof(PharmacyMedicamentOrderItem.OrderId)}]
                ,[OrderCount]       [{nameof(PharmacyMedicamentOrderItem.OrderCount)}]
                ,[StatusId]         [{nameof(PharmacyMedicamentOrderItem.Status)}]
                ,[OrderedAt]        [{nameof(PharmacyMedicamentOrderItem.OrderedAt)}]
                ,[UpdatedAt]        [{nameof(PharmacyMedicamentOrderItem.UpdatedAt)}]
            FROM PharmacyMedicamentOrders
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<PharmacyMedicamentOrderItem>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<PharmacyMedicamentOrderItem>()
        );
    }
}