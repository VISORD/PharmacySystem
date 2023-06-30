using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IPharmacyMedicamentSaleRepository
{
    Task<ItemsPagingResult<PharmacyMedicamentSale>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        PharmacyMedicamentSaleItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        IDbTransaction transaction,
        PharmacyMedicamentSale sale,
        CancellationToken cancellationToken = default
    );
}

public sealed class PharmacyMedicamentSaleRepository : IPharmacyMedicamentSaleRepository
{
    public async Task<ItemsPagingResult<PharmacyMedicamentSale>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        PharmacyMedicamentSaleItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var (filters, parameters) = request.SqlFiltering();
        var where = "[PharmacyId] = @PharmacyId AND [MedicamentId] = @MedicamentId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("PharmacyId", "MedicamentId", "SoldAt"));

        var @params = new DynamicParameters();
        @params.Add("PharmacyId", pharmacyId);
        @params.Add("MedicamentId", medicamentId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
            SELECT COUNT (*)
            FROM [pharmacy].[PharmacyMedicamentSale]
            WHERE {where};

            SELECT
                 [PharmacyId]     [{nameof(PharmacyMedicamentSale.PharmacyId)}]
                ,[MedicamentId]   [{nameof(PharmacyMedicamentSale.MedicamentId)}]
                ,[SoldAt]         [{nameof(PharmacyMedicamentSale.SoldAt)}]
                ,[SalePrice]      [{nameof(PharmacyMedicamentSale.SalePrice)}]
                ,[UnitsSold]      [{nameof(PharmacyMedicamentSale.UnitsSold)}]
            FROM [pharmacy].[PharmacyMedicamentSale]
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<PharmacyMedicamentSale>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<PharmacyMedicamentSale>()
        );
    }

    public async Task AddAsync(
        IDbTransaction transaction,
        PharmacyMedicamentSale sale,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            INSERT INTO [pharmacy].[PharmacyMedicamentSale] (
                 [PharmacyId]
                ,[MedicamentId]
                ,[SoldAt]
                ,[SalePrice]
                ,[UnitsSold]
            )
            VALUES (
                 @{nameof(PharmacyMedicamentSale.PharmacyId)}
                ,@{nameof(PharmacyMedicamentSale.MedicamentId)}
                ,@{nameof(PharmacyMedicamentSale.SoldAt)}
                ,@{nameof(PharmacyMedicamentSale.SalePrice)}
                ,@{nameof(PharmacyMedicamentSale.UnitsSold)}
            );
        ", parameters: sale, transaction: transaction, cancellationToken: cancellationToken));
    }
}