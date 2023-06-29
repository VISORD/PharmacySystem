using System.Data;
using Dapper;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Database.Parameters;

namespace PharmacySystem.WebAPI.Database.Repositories;

public interface IPharmacyWorkingHoursRepository
{
    Task<IEnumerable<PharmacyWorkingHours>> GetAsync(
        IDbTransaction transaction,
        int pharmacyId,
        CancellationToken cancellationToken = default
    );

    Task MergeAsync(
        IDbTransaction transaction,
        int pharmacyId,
        IEnumerable<PharmacyWorkingHours> workingHours,
        CancellationToken cancellationToken = default
    );
}

public sealed class PharmacyWorkingHoursRepository : IPharmacyWorkingHoursRepository
{
    public async Task<IEnumerable<PharmacyWorkingHours>> GetAsync(
        IDbTransaction transaction,
        int pharmacyId,
        CancellationToken cancellationToken = default
    )
    {
        return await transaction.Connection.QueryAsync<PharmacyWorkingHours>(new CommandDefinition($@"
            SELECT
                 [PharmacyId]   [{nameof(PharmacyWorkingHours.PharmacyId)}]
                ,[Weekday]      [{nameof(PharmacyWorkingHours.Weekday)}]
                ,[StartTime]    [{nameof(PharmacyWorkingHours.StartTime)}]
                ,[StopTime]     [{nameof(PharmacyWorkingHours.StopTime)}]
            FROM [pharmacy].[PharmacyWorkingHours]
            WHERE [PharmacyId] = @PharmacyId;
        ", parameters: new { PharmacyId = pharmacyId }, transaction: transaction, cancellationToken: cancellationToken));
    }

    public async Task MergeAsync(
        IDbTransaction transaction,
        int pharmacyId,
        IEnumerable<PharmacyWorkingHours> workingHours,
        CancellationToken cancellationToken = default
    )
    {
        await transaction.Connection.ExecuteAsync(new CommandDefinition($@"
            MERGE INTO [pharmacy].[PharmacyWorkingHours] t
            USING @WorkingHours s ON t.[PharmacyId] = s.[PharmacyId] AND t.[Weekday] = s.[Weekday]
            WHEN MATCHED THEN
                UPDATE SET
                     [StartTime] = s.[StartTime]
                    ,[StopTime] = s.[StopTime]
            WHEN NOT MATCHED THEN
                INSERT (
                     [PharmacyId]
                    ,[Weekday]
                    ,[StartTime]
                    ,[StopTime]
                )
                VALUES (
                     s.[PharmacyId]
                    ,s.[Weekday]
                    ,s.[StartTime]
                    ,s.[StopTime]
                )
            WHEN NOT MATCHED BY SOURCE AND t.[PharmacyId] = @PharmacyId THEN
                DELETE;
        ", parameters: new
        {
            PharmacyId = pharmacyId,
            WorkingHours = workingHours.ToTableValuedParameter()
        }, transaction: transaction, cancellationToken: cancellationToken));
    }
}