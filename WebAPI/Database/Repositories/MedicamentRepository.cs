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

    Task AddAsync(
        IDbTransaction transaction,
        Medicament medicament,
        CancellationToken cancellationToken = default
    );

    Task<Medicament?> GetAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        IDbTransaction transaction,
        Medicament medicament,
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
        var (filters, parameters) = request.SqlFiltering<Medicament>();
        var where = "[CompanyId] = @CompanyId" + (filters.Count > 0 ? $" AND {string.Join(" AND ", filters)}" : "");
        var orderBy = string.Join(", ", request.SqlOrdering("Id"));

        var @params = new DynamicParameters();
        @params.Add("CompanyId", companyId);
        foreach (var (field, value) in parameters)
        {
            @params.Add(field, value);
        }

        await using var reader = await transaction.Connection.QueryMultipleAsync(new CommandDefinition($@"
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
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<Medicament>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<Medicament>()
        );
    }

    public async Task AddAsync(
        IDbTransaction transaction,
        Medicament medicament,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            INSERT INTO [medicament].[Medicament] (
                 [CompanyId]
                ,[Name]
                ,[Description]
                ,[VendorPrice]
            )
            VALUES (
                 @{nameof(Medicament.CompanyId)}
                ,@{nameof(Medicament.Name)}
                ,@{nameof(Medicament.Description)}
                ,@{nameof(Medicament.VendorPrice)}
            );
        ", parameters: medicament, transaction: transaction, cancellationToken: cancellationToken));
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
        Medicament medicament,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            UPDATE [medicament].[Medicament]
            SET
                 [Name]        = @{nameof(Medicament.Name)}
                ,[Description] = @{nameof(Medicament.Description)}
                ,[VendorPrice] = @{nameof(Medicament.VendorPrice)}
            FROM [medicament].[Medicament]
            WHERE [Id] = @{nameof(Medicament.Id)};
        ", parameters: medicament, transaction: transaction, cancellationToken: cancellationToken));
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