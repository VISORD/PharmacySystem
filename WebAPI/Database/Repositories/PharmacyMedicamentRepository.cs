using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IPharmacyMedicamentRepository
{
    Task<bool> IsExistAsync(
        IDbTransaction transaction,
        int companyId,
        int pharmacyId,
        int medicamentId,
        CancellationToken cancellationToken = default
    );

    Task<ItemsPagingResult<PharmacyMedicamentItem>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        PharmacyMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        IDbTransaction transaction,
        PharmacyMedicament pharmacyMedicament,
        CancellationToken cancellationToken = default
    );

    Task<PharmacyMedicamentProfile?> GetProfileAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default
    );

    Task<PharmacyMedicament?> GetAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        CancellationToken cancellationToken = default
    );
}

public sealed class PharmacyMedicamentRepository : IPharmacyMedicamentRepository
{
    public async Task<bool> IsExistAsync(
        IDbTransaction transaction,
        int companyId,
        int pharmacyId,
        int medicamentId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [pharmacy].[PharmacyMedicament] pm
            JOIN [pharmacy].[Pharmacy] p ON p.[Id] = pm.[PharmacyId]
            WHERE p.[CompanyId] = @CompanyId AND p.[Id] = @PharmacyId AND pm.[MedicamentId] = @MedicamentId;
        ", parameters: new
        {
            CompanyId = companyId,
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId,
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<ItemsPagingResult<PharmacyMedicamentItem>> ListAsync(
        IDbTransaction transaction,
        int pharmacyId,
        PharmacyMedicamentItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        const string with = @"WITH PharmacyMedicaments AS (
                SELECT
                     pm.[PharmacyId]
                    ,pm.[MedicamentId]
                    ,m.[Name]             [MedicamentName]
                    ,m.[VendorPrice]      [VendorPrice]
                    ,r.[RetailPrice]
                    ,r.[StartDate]        [RateStartDate]
                    ,r.[StopDate]         [RateStopDate]
                    ,pm.[QuantityOnHand]
                FROM [pharmacy].[PharmacyMedicament] pm
                JOIN [pharmacy].[Pharmacy] p ON p.[Id] = pm.[PharmacyId]
                JOIN [medicament].[Medicament] m ON m.[Id] = pm.[MedicamentId]
                LEFT JOIN [pharmacy].[PharmacyMedicamentRate] r ON r.[PharmacyId] = pm.[PharmacyId] AND r.[MedicamentId] = pm.[MedicamentId]
                WHERE @AsOfDate BETWEEN r.[StartDate] AND r.[StopDate]
            )";
        var (filters, parameters) = request.SqlFiltering<PharmacyMedicamentItem>();
        var where = "[PharmacyId] = @PharmacyId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("PharmacyId", "MedicamentId"));

        var @params = new DynamicParameters();
        @params.Add("PharmacyId", pharmacyId);
        @params.Add("AsOfDate", request.AsOfDate, DbType.Date);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
            {with}
            SELECT COUNT (*)
            FROM PharmacyMedicaments
            WHERE {where};

            {with}
            SELECT
                 [PharmacyId]        [{nameof(PharmacyMedicamentItem.PharmacyId)}]
                ,[MedicamentId]      [{nameof(PharmacyMedicamentItem.MedicamentId)}]
                ,[MedicamentName]    [{nameof(PharmacyMedicamentItem.MedicamentName)}]
                ,[VendorPrice]       [{nameof(PharmacyMedicamentItem.VendorPrice)}]
                ,[RetailPrice]       [{nameof(PharmacyMedicamentItem.RetailPrice)}]
                ,[RateStartDate]     [{nameof(PharmacyMedicamentItem.RateStartDate)}]
                ,[RateStopDate]      [{nameof(PharmacyMedicamentItem.RateStopDate)}]
                ,[QuantityOnHand]    [{nameof(PharmacyMedicamentItem.QuantityOnHand)}]
            FROM PharmacyMedicaments
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<PharmacyMedicamentItem>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<PharmacyMedicamentItem>()
        );
    }

    public async Task AddAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            INSERT INTO [pharmacy].[PharmacyMedicament] (
                 [PharmacyId]
                ,[MedicamentId]
            )
            VALUES (
                 @PharmacyId
                ,@MedicamentId
            );
        ", parameters: new
        {
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        IDbTransaction transaction,
        PharmacyMedicament pharmacyMedicament,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            UPDATE [pharmacy].[PharmacyMedicament]
            SET [QuantityOnHand] = @{nameof(PharmacyMedicament.QuantityOnHand)}
            FROM [pharmacy].[PharmacyMedicament]
            WHERE [PharmacyId] = @{nameof(PharmacyMedicament.PharmacyId)} AND [MedicamentId] = @{nameof(PharmacyMedicament.MedicamentId)};
        ", parameters: pharmacyMedicament, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<PharmacyMedicamentProfile?> GetProfileAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<PharmacyMedicamentProfile>(new CommandDefinition($@"
            SELECT
                 pm.[PharmacyId]      [{nameof(PharmacyMedicamentProfile.PharmacyId)}]
                ,p.[Name]             [{nameof(PharmacyMedicamentProfile.PharmacyName)}]
                ,p.[Address]          [{nameof(PharmacyMedicamentProfile.PharmacyAddress)}]
                ,pm.[MedicamentId]    [{nameof(PharmacyMedicamentProfile.MedicamentId)}]
                ,m.[Name]             [{nameof(PharmacyMedicamentProfile.MedicamentName)}]
                ,m.[VendorPrice]      [{nameof(PharmacyMedicamentProfile.VendorPrice)}]
                ,r.[RetailPrice]      [{nameof(PharmacyMedicamentProfile.RetailPrice)}]
                ,r.[StartDate]        [{nameof(PharmacyMedicamentProfile.RateStartDate)}]
                ,r.[StopDate]         [{nameof(PharmacyMedicamentProfile.RateStopDate)}]
                ,pm.[QuantityOnHand]  [{nameof(PharmacyMedicamentProfile.QuantityOnHand)}]
            FROM [pharmacy].[PharmacyMedicament] pm
            JOIN [pharmacy].[Pharmacy] p ON p.[Id] = pm.[PharmacyId]
            JOIN [medicament].[Medicament] m ON m.[Id] = pm.[MedicamentId]
            LEFT JOIN [pharmacy].[PharmacyMedicamentRate] r ON r.[PharmacyId] = pm.[PharmacyId] AND r.[MedicamentId] = pm.[MedicamentId]
            WHERE pm.[PharmacyId] = @PharmacyId
              AND pm.[MedicamentId] = @MedicamentId
              AND @AsOfDate BETWEEN r.[StartDate] AND r.[StopDate];
        ", parameters: new
        {
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId,
            AsOfDate = asOfDate ?? DateTime.Today
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<PharmacyMedicament?> GetAsync(
        IDbTransaction transaction,
        int pharmacyId,
        int medicamentId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<PharmacyMedicament>(new CommandDefinition($@"
            SELECT
                 pm.[PharmacyId]      [{nameof(PharmacyMedicamentProfile.PharmacyId)}]
                ,pm.[MedicamentId]    [{nameof(PharmacyMedicamentProfile.MedicamentId)}]
                ,pm.[QuantityOnHand]  [{nameof(PharmacyMedicamentProfile.QuantityOnHand)}]
            FROM [pharmacy].[PharmacyMedicament] pm
            WHERE pm.[PharmacyId] = @PharmacyId
              AND pm.[MedicamentId] = @MedicamentId;
        ", parameters: new
        {
            PharmacyId = pharmacyId,
            MedicamentId = medicamentId
        }, transaction: transaction, cancellationToken: cancellationToken));
    }
}