using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using PharmacySystem.WebAPI.Options;

namespace PharmacySystem.WebAPI.Database.Connection;

public sealed class DatabaseConnectionProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.BindingInfo.BindingSource is not null &&
            context.BindingInfo.BindingSource.CanAcceptDataFrom(CustomBindingSources.DatabaseConnection) &&
            context.Metadata is DefaultModelMetadata metadata)
        {
            var databaseContextAttribute = (DatabaseAttribute?) metadata.Attributes.ParameterAttributes?.FirstOrDefault(x => x is DatabaseAttribute);
            if (databaseContextAttribute is not null && context.Metadata.ModelType.IsAssignableTo(typeof(SqlConnection)))
            {
                var applicationOptions = context.Services.GetRequiredService<IOptions<ApplicationOptions>>().Value;
                return new DatabaseConnectionBinder(applicationOptions, databaseContextAttribute);
            }
        }

        return null;
    }
}