using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dac;
using PharmacySystem.WebAPI.Extensions;
using PharmacySystem.WebAPI.Options;

namespace PharmacySystem.WebAPI.Database.Publisher;

public sealed class DatabasePublicationService : IDatabasePublicationService
{
    private readonly ILogger<DatabasePublicationService> _logger;
    private readonly IApplicationOptions _options;
    private readonly DatabaseContext _databaseContext;

    public DatabasePublicationService(
        ILogger<DatabasePublicationService> logger,
        IOptions<ApplicationOptions> options,
        DatabaseContext databaseContext
    )
    {
        _logger = logger;
        _options = options.Value;
        _databaseContext = databaseContext;
    }

    public DatabasePublicationResult Publish(DatabasePublicationOptions options)
    {
        try
        {
            var result = PublishDatabase(options);
            AllowSnapshotIsolation();

            return DatabasePublicationResult.Succeed(result);
        }
        catch (Exception exception)
        {
            return DatabasePublicationResult.Failed(exception);
        }
    }

    private PublishResult PublishDatabase(DatabasePublicationOptions options)
    {
        _logger.LogInformation("[DB: {Target}] [1] […] Database publishing", _databaseContext.DatabaseName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var dacPackage = DacPackage.Load(options.DacpacFilePath);

            var dacService = new DacServices(_options.DbConnectionString);
            if (options.PrintMessages)
            {
                dacService.Message += (_, args) =>
                {
                    // Print each messages from DB
                    _logger.LogInformation("[DB: {Target}]  *  [-] {Message}", _databaseContext.DatabaseName, args.Message.Message);
                };
            }

            var publishOptions = PreparePublishOptions(options);
            var result = dacService.Publish(dacPackage, _databaseContext.DatabaseName, publishOptions);

            PrintResult(result);

            _logger.LogInformation("[DB: {Target}] [1] [✓] Publishing was completed (elapsed: {TimeElapsed})", _databaseContext.DatabaseName, stopwatch.FormatElapsed());

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DB: {Target}] [1] [✗] Failed to publish database (elapsed: {TimeElapsed})", _databaseContext.DatabaseName, stopwatch.FormatElapsed());
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private void AllowSnapshotIsolation()
    {
        _logger.LogInformation("[DB: {Target}] [2] […] Altering database to allow snapshot isolation", _databaseContext.DatabaseName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            _databaseContext.Database.ExecuteSqlRawAsync($"ALTER DATABASE [{_databaseContext.DatabaseName}] SET ALLOW_SNAPSHOT_ISOLATION ON;");

            _logger.LogInformation("[DB: {Target}] [2] [✓] Setting was applied successfully (elapsed: {TimeElapsed})", _databaseContext.DatabaseName, stopwatch.FormatElapsed());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[DB: {Target}] [2] [✗] Failed to modify setting (elapsed: {TimeElapsed})", _databaseContext.DatabaseName, stopwatch.FormatElapsed());
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private static PublishOptions PreparePublishOptions(DatabasePublicationOptions options)
    {
        var dacDeployOptions = new DacDeployOptions
        {
            BlockOnPossibleDataLoss = options.BlockOnPossibleDataLoss,

            DisableAndReenableDdlTriggers = options.DisableAndReenableDdlTriggers,
            DoNotAlterChangeDataCaptureObjects = options.DoNotAlterChangeDataCaptureObjects,
            DoNotAlterReplicatedObjects = options.DoNotAlterReplicatedObjects,
            DoNotEvaluateSqlCmdVariables = options.DoNotEvaluateSqlCmdVariables,
            DropStatisticsNotInSource = options.DropStatisticsNotInSource,
            IncludeCompositeObjects = options.IncludeCompositeObjects,
            PopulateFilesOnFileGroups = options.PopulateFilesOnFileGroups,
            RestoreSequenceCurrentValue = options.RestoreSequenceCurrentValue,
            ScriptRefreshModule = options.ScriptRefreshModule,
            ScriptNewConstraintValidation = options.ScriptNewConstraintValidation,
            DisableIndexesForDataPhase = options.DisableIndexesForDataPhase,
            UnmodifiableObjectWarnings = options.UnmodifiableObjectWarnings,
            VerifyCollationCompatibility = options.VerifyCollationCompatibility,
            VerifyDeployment = options.VerifyDeployment,

            DropConstraintsNotInSource = options.DropConstraintsNotInSource,
            DropDmlTriggersNotInSource = options.DropDmlTriggersNotInSource,
            DropExtendedPropertiesNotInSource = options.DropExtendedPropertiesNotInSource,
            DropIndexesNotInSource = options.DropIndexesNotInSource,

            IgnoreAnsiNulls = options.IgnoreAnsiNulls,
            IgnoreFileAndLogFilePath = options.IgnoreFileAndLogFilePath,
            IgnoreFileSize = options.IgnoreFileSize,
            IgnoreFilegroupPlacement = options.IgnoreFilegroupPlacement,
            IgnoreFillFactor = options.IgnoreFillFactor,
            IgnoreFullTextCatalogFilePath = options.IgnoreFullTextCatalogFilePath,
            IgnoreIndexPadding = options.IgnoreIndexPadding,
            IgnoreKeywordCasing = options.IgnoreKeywordCasing,
            IgnoreLoginSids = options.IgnoreLoginSids,
            IgnoreObjectPlacementOnPartitionScheme = options.IgnoreObjectPlacementOnPartitionScheme,
            IgnoreQuotedIdentifiers = options.IgnoreQuotedIdentifiers,
            IgnoreRouteLifetime = options.IgnoreRouteLifetime,
            IgnoreSemicolonBetweenStatements = options.IgnoreSemicolonBetweenStatements,
            IgnoreWhitespace = options.IgnoreWhitespace,
            IgnoreColumnOrder = options.IgnoreColumnOrder,

            CommandTimeout = options.CommandTimeout,
        };

        foreach (var (sqlCommand, value) in options.SqlCommandVariableValues)
        {
            dacDeployOptions.SetVariable(sqlCommand, value);
        }

        return new PublishOptions
        {
            DeployOptions = dacDeployOptions,
            GenerateDeploymentReport = false,
            GenerateDeploymentScript = false,
        };
    }

    private void PrintResult(PublishResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.DeploymentReport))
        {
            const string message = "[DB: {Target}]  *  [-] Deployment report:{NewLine}{Report}";
            _logger.LogInformation(message, _databaseContext.DatabaseName, Environment.NewLine, result.DeploymentReport);
        }

        if (!string.IsNullOrWhiteSpace(result.DatabaseScript))
        {
            const string message = "[DB: {Target}]  *  [-] Deployment script:{NewLine}{Script}";
            _logger.LogInformation(message, _databaseContext.DatabaseName, Environment.NewLine, result.DatabaseScript);
        }
    }
}