using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Medicament;
using PharmacySystem.WebAPI.Models.Medicament;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IMedicamentRepository
{
    Task<bool> IsNameDuplicatedAsync(IDbTransaction transaction,
        int companyId,
        string name,
        int? id = null,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsExistAsync(IDbTransaction transaction,
        int companyId,
        int id,
        CancellationToken cancellationToken = default
    );

    Task<ItemsPagingResult<Medicament>> ListAsync(
        IDbTransaction transaction,
        int companyId,
        MedicamentItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task<int> AddAsync(
        IDbTransaction transaction,
        int companyId,
        MedicamentModification medicament,
        CancellationToken cancellationToken = default
    );

    Task<Medicament?> GetAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        IDbTransaction transaction,
        int id,
        MedicamentModification medicament,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );
}

public sealed class MedicamentRepository : IMedicamentRepository
{
    public async Task<bool> IsNameDuplicatedAsync(
        IDbTransaction transaction,
        int companyId,
        string name,
        int? id = null,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [medicament].[Medicament]
            WHERE [CompanyId] = @CompanyId AND [Name] = @Name AND (@Id IS NULL OR [Id] <> @Id);
        ", parameters: new
        {
            CompanyId = companyId,
            Name = name,
            Id = id
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<bool> IsExistAsync(
        IDbTransaction transaction,
        int companyId,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [medicament].[Medicament]
            WHERE [CompanyId] = @CompanyId AND [Id] = @Id;
        ", parameters: new
        {
            CompanyId = companyId,
            Id = id
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<ItemsPagingResult<Medicament>> ListAsync(
        IDbTransaction transaction,
        int companyId,
        MedicamentItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var (filters, parameters) = request.SqlFiltering();
        var orderBy = string.Join(", ", request.SqlOrdering("Id"));

        string query;
        if (request.ExcludeById is not null)
        {
            const string with = @"WITH Excludes AS (
                SELECT
                     a.[OriginalId]
                    ,a.[AnalogueId]
                FROM [medicament].[Medicament] m
                LEFT JOIN [medicament].[MedicamentAnalogue] a ON m.[Id] = a.[OriginalId]
                WHERE a.[AnalogueId] IS NOT NULL
            ), Medicaments AS (
                SELECT
                     m.[Id]
                    ,m.[CompanyId]
                    ,m.[Name]
                    ,m.[Description]
                    ,m.[VendorPrice]
                    ,e.[OriginalId]
                FROM [medicament].[Medicament] m
                LEFT JOIN Excludes e ON e.[AnalogueId] = m.[Id]
            )";

            var where = "[CompanyId] = @CompanyId AND [Id] <> @ExcludeById AND ([OriginalId] <> @ExcludeById OR [OriginalId] IS NULL)"
                        + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");

            query = $@"
                {with}
                SELECT COUNT (*)
                FROM Medicaments
                WHERE {where};

                {with}
                SELECT
                     [Id]            [{nameof(Medicament.Id)}]
                    ,[CompanyId]     [{nameof(Medicament.CompanyId)}]
                    ,[Name]          [{nameof(Medicament.Name)}]
                    ,[Description]   [{nameof(Medicament.Description)}]
                    ,[VendorPrice]   [{nameof(Medicament.VendorPrice)}]
                FROM Medicaments
                WHERE {where}
                GROUP BY
                     [Id]
                    ,[CompanyId]
                    ,[Name]
                    ,[Description]
                    ,[VendorPrice]
                ORDER BY {orderBy}
                OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
            ";
        }
        else if (request.ExcludeByOrderId is not null)
        {
            const string with = @"WITH Excludes AS (
                SELECT DISTINCT m.[Id]
                FROM [medicament].[Medicament] m
                LEFT JOIN [order].[OrderMedicament] om ON m.[Id] = om.[MedicamentId]
                WHERE om.[OrderId] = @ExcludeByOrderId
            ), Medicaments AS (
                SELECT
                     m.[Id]
                    ,m.[CompanyId]
                    ,m.[Name]
                    ,m.[Description]
                    ,m.[VendorPrice]
                FROM [medicament].[Medicament] m
                LEFT JOIN Excludes e ON e.[Id] = m.[Id]
                WHERE e.[Id] IS NULL
            )";

            var where = "[CompanyId] = @CompanyId"
                        + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");

            query = $@"
                {with}
                SELECT COUNT (*)
                FROM Medicaments
                WHERE {where};

                {with}
                SELECT
                     [Id]            [{nameof(Medicament.Id)}]
                    ,[CompanyId]     [{nameof(Medicament.CompanyId)}]
                    ,[Name]          [{nameof(Medicament.Name)}]
                    ,[Description]   [{nameof(Medicament.Description)}]
                    ,[VendorPrice]   [{nameof(Medicament.VendorPrice)}]
                FROM Medicaments
                WHERE {where}
                GROUP BY
                     [Id]
                    ,[CompanyId]
                    ,[Name]
                    ,[Description]
                    ,[VendorPrice]
                ORDER BY {orderBy}
                OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
            ";
        }
        else
        {
            var where = "[CompanyId] = @CompanyId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");

            query = $@"
                SELECT COUNT (*)
                FROM [medicament].[Medicament]
                WHERE {where};

                SELECT
                     [Id]            [{nameof(Medicament.Id)}]
                    ,[CompanyId]     [{nameof(Medicament.CompanyId)}]
                    ,[Name]          [{nameof(Medicament.Name)}]
                    ,[Description]   [{nameof(Medicament.Description)}]
                    ,[VendorPrice]   [{nameof(Medicament.VendorPrice)}]
                FROM [medicament].[Medicament]
                WHERE {where}
                ORDER BY {orderBy}
                OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
            ";
        }

        var @params = new DynamicParameters();
        @params.Add("CompanyId", companyId);
        @params.Add("ExcludeById", request.ExcludeById);
        @params.Add("ExcludeByOrderId", request.ExcludeByOrderId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition(query, parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<Medicament>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<Medicament>()
        );
    }

    public async Task<int> AddAsync(
        IDbTransaction transaction,
        int companyId,
        MedicamentModification medicament,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleAsync<int>(new CommandDefinition($@"
            DECLARE @Id [core].[IntegerItem];

            INSERT INTO [medicament].[Medicament] (
                 [CompanyId]
                ,[Name]
                ,[Description]
                ,[VendorPrice]
            )
            OUTPUT INSERTED.[Id] INTO @Id
            VALUES (
                 @CompanyId
                ,@{nameof(MedicamentModification.Name)}
                ,@{nameof(MedicamentModification.Description)}
                ,@{nameof(MedicamentModification.VendorPrice)}
            );

            SELECT [Value] FROM @Id;
        ", parameters: new
        {
            CompanyId = companyId,
            medicament.Name,
            medicament.Description,
            medicament.VendorPrice,
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<Medicament?> GetAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<Medicament>(new CommandDefinition($@"
            SELECT
                 [Id]            [{nameof(Medicament.Id)}]
                ,[CompanyId]     [{nameof(Medicament.CompanyId)}]
                ,[Name]          [{nameof(Medicament.Name)}]
                ,[Description]   [{nameof(Medicament.Description)}]
                ,[VendorPrice]   [{nameof(Medicament.VendorPrice)}]
            FROM [medicament].[Medicament]
            WHERE [Id] = @Id;
        ", parameters: new { Id = id }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        IDbTransaction transaction,
        int id,
        MedicamentModification medicament,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            UPDATE [medicament].[Medicament]
            SET
                 [Name]        = @{nameof(MedicamentModification.Name)}
                ,[Description] = @{nameof(MedicamentModification.Description)}
                ,[VendorPrice] = @{nameof(MedicamentModification.VendorPrice)}
            FROM [medicament].[Medicament]
            WHERE [Id] = @Id;
        ", parameters: new
        {
            Id = id,
            medicament.Name,
            medicament.Description,
            medicament.VendorPrice,
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            DELETE FROM [medicament].[Medicament]
            WHERE [Id] = @Id;
        ", parameters: new { Id = id }, transaction: transaction, cancellationToken: cancellationToken));
    }
}