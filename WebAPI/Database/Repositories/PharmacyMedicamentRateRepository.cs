using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Database.Parameters;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IPharmacyMedicamentRateRepository
{
    Task<ItemsPagingResult<PharmacyMedicamentRate>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        PharmacyMedicamentRateItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task<PharmacyMedicamentRate?> GetAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<PharmacyMedicamentRate>> GetIntersectedAsync(
        IDbTransaction transaction,
        PharmacyMedicamentRate rate,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        IDbTransaction transaction,
        IEnumerable<PharmacyMedicamentRate> rates,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        IDbTransaction transaction,
        IEnumerable<PharmacyMedicamentRate> rates,
        CancellationToken cancellationToken = default
    );
}

public sealed class PharmacyMedicamentRateRepository : IPharmacyMedicamentRateRepository
{
    public async Task<ItemsPagingResult<PharmacyMedicamentRate>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        PharmacyMedicamentRateItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var (filters, parameters) = request.SqlFiltering<PharmacyMedicamentRate>();
        var where = "[PharmacyId] = @PharmacyId AND [MedicamentId] = @MedicamentId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("PharmacyId", "MedicamentId", "StartDate", "StopDate"));

        var @params = new DynamicParameters();
        @params.Add("PharmacyId", pharmacyId);
        @params.Add("MedicamentId", medicamentId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
            SELECT COUNT (*)
            FROM [pharmacy].[PharmacyMedicamentRate]
            WHERE {where};

            SELECT
                 [PharmacyId]     [{nameof(PharmacyMedicamentRate.PharmacyId)}]
                ,[MedicamentId]   [{nameof(PharmacyMedicamentRate.MedicamentId)}]
                ,[StartDate]      [{nameof(PharmacyMedicamentRate.StartDate)}]
                ,[StopDate]       [{nameof(PharmacyMedicamentRate.StopDate)}]
                ,[RetailPrice]    [{nameof(PharmacyMedicamentRate.RetailPrice)}]
            FROM [pharmacy].[PharmacyMedicamentRate]
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<PharmacyMedicamentRate>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<PharmacyMedicamentRate>()
        );
    }

    public async Task<PharmacyMedicamentRate?> GetAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<PharmacyMedicamentRate>(new CommandDefinition($@"
            SELECT
                 [PharmacyId]     [{nameof(PharmacyMedicamentRate.PharmacyId)}]
                ,[MedicamentId]   [{nameof(PharmacyMedicamentRate.MedicamentId)}]
                ,[StartDate]      [{nameof(PharmacyMedicamentRate.StartDate)}]
                ,[StopDate]       [{nameof(PharmacyMedicamentRate.StopDate)}]
                ,[RetailPrice]    [{nameof(PharmacyMedicamentRate.RetailPrice)}]
            FROM [pharmacy].[PharmacyMedicamentRate]
            WHERE [PharmacyId]   =  @{nameof(PharmacyMedicamentRate.PharmacyId)}
              AND [MedicamentId] =  @{nameof(PharmacyMedicamentRate.MedicamentId)}
              AND @AsOfDate BETWEEN [StartDate] AND [StopDate];
        ", parameters: new
        {
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId,
            AsOfDate = asOfDate ?? DateTime.Today,
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<PharmacyMedicamentRate>> GetIntersectedAsync(
        IDbTransaction transaction,
        PharmacyMedicamentRate rate,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QueryAsync<PharmacyMedicamentRate>(new CommandDefinition($@"
            SELECT
                 [PharmacyId]     [{nameof(PharmacyMedicamentRate.PharmacyId)}]
                ,[MedicamentId]   [{nameof(PharmacyMedicamentRate.MedicamentId)}]
                ,[StartDate]      [{nameof(PharmacyMedicamentRate.StartDate)}]
                ,[StopDate]       [{nameof(PharmacyMedicamentRate.StopDate)}]
                ,[RetailPrice]    [{nameof(PharmacyMedicamentRate.RetailPrice)}]
            FROM [pharmacy].[PharmacyMedicamentRate]
            WHERE [PharmacyId]   =  @{nameof(PharmacyMedicamentRate.PharmacyId)}
              AND [MedicamentId] =  @{nameof(PharmacyMedicamentRate.MedicamentId)}
              AND [StartDate]    <= @{nameof(PharmacyMedicamentRate.StartDate)}
              AND [StopDate]     >= @{nameof(PharmacyMedicamentRate.RetailPrice)};
        ", parameters: rate, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(
        IDbTransaction transaction,
        IEnumerable<PharmacyMedicamentRate> rates,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            INSERT INTO [pharmacy].[PharmacyMedicamentRate] (
                 [PharmacyId]
                ,[MedicamentId]
                ,[StartDate]
                ,[StopDate]
                ,[RetailPrice]
            )
            SELECT
                 [PharmacyId]
                ,[MedicamentId]
                ,[StartDate]
                ,[StopDate]
                ,[RetailPrice]
            FROM @Rates;
        ", parameters: new { Rates = rates.ToTableValuedParameter() }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(
        IDbTransaction transaction,
        IEnumerable<PharmacyMedicamentRate> rates,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            DELETE t
            FROM [pharmacy].[PharmacyMedicamentRate] t
            JOIN @Rates s ON s.[PharmacyId] = t.[PharmacyId] AND s.[MedicamentId] = t.[MedicamentId] AND s.[StartDate] = t.[StartDate] AND s.[StopDate] = t.[StopDate];
        ", parameters: new { Rates = rates.ToTableValuedParameter() }, transaction: transaction, cancellationToken: cancellationToken));
    }
}