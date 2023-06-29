using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Entities.Company;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IAccountRepository
{
    Task<bool> IsEmailDuplicatedAsync(
        IDbTransaction transaction,
        string email,
        int? companyId = null,
        CancellationToken cancellationToken = default
    );

    Task<Company?> SignInAsync(
        IDbTransaction transaction,
        string email,
        string password,
        CancellationToken cancellationToken = default
    );

    Task<Company> SignUpAsync(
        IDbTransaction transaction,
        string email,
        string name,
        string password,
        CancellationToken cancellationToken = default
    );
}

public sealed class AccountRepository : IAccountRepository
{
    public async Task<bool> IsEmailDuplicatedAsync(
        IDbTransaction transaction,
        string email,
        int? companyId = null,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleOrDefaultAsync<bool>(new CommandDefinition(@"
            SELECT TOP 1 1
            FROM [company].[Company]
            WHERE [Email] = @Email AND (@Id IS NULL OR [Id] <> @Id);
        ", parameters: new
        {
            Email = email,
            Id = companyId
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<Company?> SignInAsync(
        IDbTransaction transaction,
        string email,
        string password,
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
            WHERE [Email] = @Email AND [Password] = @Password;
        ", parameters: new
        {
            Email = email,
            Password = password
        }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task<Company> SignUpAsync(
        IDbTransaction transaction,
        string email,
        string name,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QuerySingleAsync<Company>(new CommandDefinition($@"
            DECLARE @Id [core].[IntegerItem];

            INSERT INTO [company].[Company] (
                 [Email]
                ,[Name]
                ,[Password]
            )
            OUTPUT INSERTED.[Id] INTO @Id
            VALUES (
                 @Email
                ,@Name
                ,@Password
            );

            SELECT
                 c.[Id]      [{nameof(Company.Id)}]
                ,c.[Email]   [{nameof(Company.Email)}]
                ,c.[Name]    [{nameof(Company.Name)}]
                ,c.[Phone]   [{nameof(Company.Phone)}]
            FROM [company].[Company] c
            JOIN @Id i ON i.[Value] = c.[Id];
        ", parameters: new
        {
            Email = email,
            Name = name,
            Password = password
        }, transaction: transaction, cancellationToken: cancellationToken));
    }
}