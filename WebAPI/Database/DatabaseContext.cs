using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PharmacySystem.WebAPI.Database.Entities.Company;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Options;

namespace PharmacySystem.WebAPI.Database;

public sealed class DatabaseContext : DbContext
{
    private readonly IApplicationOptions _options;

    public string DatabaseName { get; }
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Pharmacy> Pharmacies { get; set; } = null!;

    public DatabaseContext(IOptions<ApplicationOptions> options)
    {
        _options = options.Value;

        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_options.DbConnectionString);
        DatabaseName = string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog)
            ? "master"
            : sqlConnectionStringBuilder.InitialCatalog;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_options.DbConnectionString);
    }
}