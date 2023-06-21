using System.Reflection;

namespace PharmacySystem.WebAPI.Database.Publisher;

public sealed class DatabasePublicationOptions
{
    private string? dacpacFilePath;

    public required string DacpacFileName { get; init; }
    public bool PrintMessages { get; init; } = true;

    public string DacpacFilePath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(dacpacFilePath))
            {
                var location = new Uri(Assembly.GetExecutingAssembly().Location);
                var projectDirectory = Path.GetDirectoryName(location.AbsolutePath) ?? Directory.GetCurrentDirectory();
                dacpacFilePath = Path.Combine(projectDirectory, DacpacFileName);
            }

            return dacpacFilePath;
        }
    }

    public bool BlockOnPossibleDataLoss { get; init; } = false;

    public bool DisableAndReenableDdlTriggers { get; init; } = true;

    public bool DoNotAlterChangeDataCaptureObjects { get; init; } = true;

    public bool DoNotAlterReplicatedObjects { get; init; } = true;

    public bool DoNotEvaluateSqlCmdVariables { get; init; } = true;

    public bool DropStatisticsNotInSource { get; init; } = true;

    public bool IncludeCompositeObjects { get; init; } = true;

    public bool PopulateFilesOnFileGroups { get; init; } = true;

    public bool RestoreSequenceCurrentValue { get; init; } = true;

    public bool ScriptRefreshModule { get; init; } = true;

    public bool ScriptNewConstraintValidation { get; init; } = true;

    public bool DisableIndexesForDataPhase { get; init; } = true;

    public bool UnmodifiableObjectWarnings { get; init; } = true;

    public bool VerifyCollationCompatibility { get; init; } = true;

    public bool VerifyDeployment { get; init; } = true;

    public bool DropConstraintsNotInSource { get; init; } = true;

    public bool DropDmlTriggersNotInSource { get; init; } = true;

    public bool DropExtendedPropertiesNotInSource { get; init; } = true;

    public bool DropIndexesNotInSource { get; init; } = true;

    public bool IgnoreAnsiNulls { get; init; } = true;

    public bool IgnoreFileAndLogFilePath { get; init; } = true;

    public bool IgnoreFileSize { get; init; } = true;

    public bool IgnoreFilegroupPlacement { get; init; } = true;

    public bool IgnoreFillFactor { get; init; } = true;

    public bool IgnoreFullTextCatalogFilePath { get; init; } = true;

    public bool IgnoreIndexPadding { get; init; } = true;

    public bool IgnoreKeywordCasing { get; init; } = true;

    public bool IgnoreLoginSids { get; init; } = true;

    public bool IgnoreObjectPlacementOnPartitionScheme { get; init; } = true;

    public bool IgnoreQuotedIdentifiers { get; init; } = true;

    public bool IgnoreRouteLifetime { get; init; } = true;

    public bool IgnoreSemicolonBetweenStatements { get; init; } = true;

    public bool IgnoreWhitespace { get; init; } = true;

    public bool IgnoreColumnOrder { get; init; } = false;

    public int CommandTimeout { get; init; } = (int) TimeSpan.FromMinutes(5).TotalSeconds;

    public Dictionary<string, string> SqlCommandVariableValues { get; init; } = new();
}