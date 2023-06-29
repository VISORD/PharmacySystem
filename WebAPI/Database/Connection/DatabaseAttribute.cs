using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PharmacySystem.WebAPI.Database.Connection;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class DatabaseAttribute : Attribute, IBindingSourceMetadata
{
    public BindingSource BindingSource => CustomBindingSources.DatabaseConnection;
    public bool IsReadOnly { get; }

    public DatabaseAttribute(bool isReadOnly = false)
    {
        IsReadOnly = isReadOnly;
    }
}