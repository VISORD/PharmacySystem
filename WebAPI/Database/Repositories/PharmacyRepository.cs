using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Common;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Models.Pharmacy;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IPharmacyRepository
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

    Task<ItemsPagingResult<PharmacyItem>> ListAsync(
        IDbTransaction transaction,
        int companyId,
        PharmacyItemsPagingRequest request,
        CancellationToken cancellationToken = default
    );

    Task<int> AddAsync(
        IDbTransaction transaction,
        int companyId,
        PharmacyModification pharmacy,
        CancellationToken cancellationToken = default
    );

    Task<Pharmacy?> GetAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        IDbTransaction transaction,
        int id,
        PharmacyModification pharmacy,
        CancellationToken cancellationToken = default
    );

    Task DeleteAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    );
}

public sealed class PharmacyRepository : IPharmacyRepository
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
            FROM [pharmacy].[Pharmacy]
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
            FROM [pharmacy].[Pharmacy]
            WHERE [CompanyId] = @CompanyId AND [Id] = @Id;
        ", parameters: new
        {
            CompanyId = companyId,
            Id = id
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<ItemsPagingResult<PharmacyItem>> ListAsync(
        IDbTransaction transaction,
        int companyId,
        PharmacyItemsPagingRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var (filters, parameters) = request.SqlFiltering();
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
            FROM [pharmacy].[Pharmacy]
            WHERE {where};

            SELECT
                 [Id]            [{nameof(PharmacyItem.Id)}]
                ,[Name]          [{nameof(PharmacyItem.Name)}]
                ,[Email]         [{nameof(PharmacyItem.Email)}]
                ,[Phone]         [{nameof(PharmacyItem.Phone)}]
                ,[Address]       [{nameof(PharmacyItem.Address)}]
            FROM [pharmacy].[Pharmacy]
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET {request.Paging.Offset} ROWS FETCH NEXT {request.Paging.Size} ROWS ONLY;
        ", parameters: @params, transaction: transaction, cancellationToken: cancellationToken));

        return new ItemsPagingResult<PharmacyItem>(
            TotalAmount: await reader.ReadSingleAsync<int>(),
            Items: await reader.ReadAsync<PharmacyItem>()
        );
    }

    public async Task<int> AddAsync(
        IDbTransaction transaction,
        int companyId,
        PharmacyModification pharmacy,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleAsync<int>(new CommandDefinition($@"
            DECLARE @Id [core].[IntegerItem];

            INSERT INTO [pharmacy].[Pharmacy] (
                 [CompanyId]
                ,[Name]
                ,[Email]
                ,[Phone]
                ,[Address]
                ,[Latitude]
                ,[Longitude]
                ,[Description]
            )
            OUTPUT INSERTED.[Id] INTO @Id
            VALUES (
                 @CompanyId
                ,@{nameof(PharmacyModification.Name)}
                ,@{nameof(PharmacyModification.Email)}
                ,@{nameof(PharmacyModification.Phone)}
                ,@{nameof(PharmacyModification.Address)}
                ,@{nameof(PharmacyModification.Latitude)}
                ,@{nameof(PharmacyModification.Longitude)}
                ,@{nameof(PharmacyModification.Description)}
            );

            SELECT [Value] FROM @Id;
        ", parameters: new
        {
            CompanyId = companyId,
            pharmacy.Name,
            pharmacy.Email,
            pharmacy.Phone,
            pharmacy.Address,
            pharmacy.Latitude,
            pharmacy.Longitude,
            pharmacy.Description,
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<Pharmacy?> GetAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<Pharmacy>(new CommandDefinition($@"
            SELECT
                 [Id]            [{nameof(Pharmacy.Id)}]
                ,[CompanyId]     [{nameof(Pharmacy.CompanyId)}]
                ,[Name]          [{nameof(Pharmacy.Name)}]
                ,[Email]         [{nameof(Pharmacy.Email)}]
                ,[Phone]         [{nameof(Pharmacy.Phone)}]
                ,[Address]       [{nameof(Pharmacy.Address)}]
                ,[Latitude]      [{nameof(Pharmacy.Latitude)}]
                ,[Longitude]     [{nameof(Pharmacy.Longitude)}]
                ,[Description]   [{nameof(Pharmacy.Description)}]
            FROM [pharmacy].[Pharmacy]
            WHERE [Id] = @Id;
        ", parameters: new { Id = id }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        IDbTransaction transaction,
        int id,
        PharmacyModification pharmacy,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            UPDATE [pharmacy].[Pharmacy]
            SET
                 [Name]        = @{nameof(PharmacyModification.Name)}
                ,[Email]       = @{nameof(PharmacyModification.Email)}
                ,[Phone]       = @{nameof(PharmacyModification.Phone)}
                ,[Address]     = @{nameof(PharmacyModification.Address)}
                ,[Latitude]    = @{nameof(PharmacyModification.Latitude)}
                ,[Longitude]   = @{nameof(PharmacyModification.Longitude)}
                ,[Description] = @{nameof(PharmacyModification.Description)}
            FROM [pharmacy].[Pharmacy]
            WHERE [Id] = @{nameof(Pharmacy.Id)};
        ", parameters: new
        {
            Id = id,
            pharmacy.Name,
            pharmacy.Email,
            pharmacy.Phone,
            pharmacy.Address,
            pharmacy.Latitude,
            pharmacy.Longitude,
            pharmacy.Description,
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(
        IDbTransaction transaction,
        int id,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            DELETE FROM [pharmacy].[Pharmacy]
            WHERE [Id] = @Id;
        ", parameters: new { Id = id }, transaction: transaction, cancellationToken: cancellationToken));
    }
}