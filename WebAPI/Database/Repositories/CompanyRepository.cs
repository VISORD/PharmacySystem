using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Entities.Company;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetAsync(
        IDbTransaction transaction,
        int companyId,
        CancellationToken cancellationToken = default
    );

    Task UpdateAsync(
        IDbTransaction transaction,
        Company company,
        CancellationToken cancellationToken = default
    );
}

public sealed class CompanyRepository : ICompanyRepository
{
    public async Task<Company?> GetAsync(
        IDbTransaction transaction,
        int companyId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<Company>(new CommandDefinition($@"
            SELECT
                 [Id]      [{nameof(Company.Id)}]
                ,[Email]   [{nameof(Company.Email)}]
                ,[Name]    [{nameof(Company.Name)}]
                ,[Phone]   [{nameof(Company.Phone)}]
            FROM [company].[Company]
            WHERE [Id] = @Id;
        ", parameters: new { Id = companyId }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task UpdateAsync(
        IDbTransaction transaction,
        Company company,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            UPDATE [company].[Company]
            SET
                 [Email] = @{nameof(Company.Email)}
                ,[Name]  = @{nameof(Company.Name)}
                ,[Phone] = @{nameof(Company.Phone)}
            FROM [company].[Company]
            WHERE [Id] = @Id;
        ", parameters: company, transaction: transaction, cancellationToken: cancellationToken));
    }
}