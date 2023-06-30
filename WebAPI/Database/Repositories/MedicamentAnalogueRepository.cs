using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Medicament;
using PharmacySystem.WebAPI.Database.Parameters;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IMedicamentAnalogueRepository
{
    Task<ItemsPagingResult<MedicamentAnalogue>> ListAsync(
        IDbTransaction transaction,
        int medicamentId,
        MedicamentAnalogueItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task AssociateAsync(
        IDbTransaction transaction,
        int medicamentId,
        IEnumerable<int> analogueIds,
        CancellationToken cancellationToken = default
    );

    Task DisassociateAsync(
        IDbTransaction transaction,
        int medicamentId,
        IEnumerable<int> analogueIds,
        CancellationToken cancellationToken = default
    );
}

public sealed class MedicamentAnalogueRepository : IMedicamentAnalogueRepository
{
    public async Task<ItemsPagingResult<MedicamentAnalogue>> ListAsync(
        IDbTransaction transaction,
        int medicamentId,
        MedicamentAnalogueItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        const string with = @"WITH Analogues AS (
                SELECT
                     [OriginalId]
                    ,[AnalogueId]
                    ,IIF(@MedicamentId = a.[OriginalId], a.[AnalogueId], a.[OriginalId])                   [Id]
                    ,IIF(@MedicamentId = a.[OriginalId], 1, 0)                                             [IsAnalogue]
                    ,IIF(@MedicamentId = a.[OriginalId], analogue.[Name], original.[Name])                 [Name]
                    ,IIF(@MedicamentId = a.[OriginalId], analogue.[VendorPrice], original.[VendorPrice])   [VendorPrice]
                FROM [medicament].[MedicamentAnalogue] a
                JOIN [medicament].[Medicament] original ON a.[OriginalId] = original.[Id]
                JOIN [medicament].[Medicament] analogue ON a.[AnalogueId] = analogue.[Id]
            )";
        var (filters, parameters) = request.SqlFiltering();
        var where = "@MedicamentId IN ([OriginalId], [AnalogueId])" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("OriginalId", "AnalogueId"));

        var @params = new DynamicParameters();
        @params.Add("MedicamentId", medicamentId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
            {with}
            SELECT COUNT (*)
            FROM Analogues
            WHERE {where};

            {with}
            SELECT
                 [Id]            [{nameof(MedicamentAnalogue.Id)}]
                ,[IsAnalogue]    [{nameof(MedicamentAnalogue.IsAnalogue)}]
                ,[Name]          [{nameof(MedicamentAnalogue.Name)}]
                ,[VendorPrice]   [{nameof(MedicamentAnalogue.VendorPrice)}]
            FROM Analogues
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<MedicamentAnalogue>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<MedicamentAnalogue>()
        );
    }

    public async Task AssociateAsync(
        IDbTransaction transaction,
        int medicamentId,
        IEnumerable<int> analogueIds,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition(@"
            INSERT INTO [medicament].[MedicamentAnalogue] (
                 [OriginalId]
                ,[AnalogueId]
            )
            SELECT
                 @MedicamentId
                ,i.[Value]
            FROM @AnalogueIds i
            LEFT JOIN [medicament].[MedicamentAnalogue] a ON a.[OriginalId] = @MedicamentId AND a.[AnalogueId] = i.[Value]
            WHERE a.[OriginalId] IS NULL;
        ", parameters: new
        {
            MedicamentId = medicamentId,
            AnalogueIds = analogueIds.ToTableValuedParameter()
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task DisassociateAsync(
        IDbTransaction transaction,
        int medicamentId,
        IEnumerable<int> analogueIds,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition(@"
            DELETE a
            FROM [medicament].[MedicamentAnalogue] a
            JOIN @AnalogueIds i ON a.[OriginalId] = @MedicamentId AND a.[AnalogueId] = i.[Value]
                                       OR a.[OriginalId] = i.[Value] AND a.[AnalogueId] = @MedicamentId;
        ", parameters: new
        {
            MedicamentId = medicamentId,
            AnalogueIds = analogueIds.ToTableValuedParameter()
        }, transaction: transaction, cancellationToken: cancellationToken));
    }
}