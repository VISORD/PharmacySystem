using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.SqlClient;
using PharmacySystem.WebAPI.Options;

namespace PharmacySystem.WebAPI.Database.Connection;

public sealed class DatabaseConnectionBinder : IModelBinder
{
    private readonly IApplicationOptions _applicationOptions;
    private readonly DatabaseAttribute _databaseAttribute;

    public DatabaseConnectionBinder(IApplicationOptions applicationOptions, DatabaseAttribute databaseAttribute)
    {
        ArgumentNullException.ThrowIfNull(databaseAttribute);

        _applicationOptions = applicationOptions;
        _databaseAttribute = databaseAttribute;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_applicationOptions.DbConnectionString)
        {
            ApplicationIntent = _databaseAttribute.IsReadOnly
                ? ApplicationIntent.ReadOnly
                : ApplicationIntent.ReadWrite
        };

        try
        {
            var connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);
            await connection.OpenAsync(bindingContext.HttpContext.RequestAborted);

            bindingContext.ValidationState.Add(connection, new ValidationStateEntry { SuppressValidation = true });
            bindingContext.Result = ModelBindingResult.Success(connection);
            bindingContext.HttpContext.Response.OnCompleted(async () =>
            {
                await using (connection)
                {
                }
            });
        }
        catch
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}