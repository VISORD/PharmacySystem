using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PharmacySystem.WebAPI.Database.Entities.Company;
using PharmacySystem.WebAPI.Database.Entities.Medicament;
using PharmacySystem.WebAPI.Database.Entities.Order;
using PharmacySystem.WebAPI.Database.Entities.Pharmacy;
using PharmacySystem.WebAPI.Options;

namespace PharmacySystem.WebAPI.Database;

public sealed class DatabaseContext : DbContext
{
    private readonly IApplicationOptions _options;

    public string DatabaseName { get; }

    #region Company

    public DbSet<Company> Companies { get; init; } = null!;

    #endregion

    #region Pharmacy

    public DbSet<Pharmacy> Pharmacies { get; init; } = null!;
    public DbSet<PharmacyMedicament> PharmacyMedicaments { get; init; } = null!;
    public DbSet<PharmacyMedicamentRate> PharmacyMedicamentRates { get; init; } = null!;
    public DbSet<PharmacyMedicamentSale> PharmacyMedicamentSales { get; init; } = null!;

    #endregion

    #region Medicament

    public DbSet<Medicament> Medicaments { get; init; } = null!;
    public DbSet<MedicamentAnalogue> MedicamentAnalogues { get; init; } = null!;

    #endregion

    #region Order

    public DbSet<Order> Orders { get; init; } = null!;
    public DbSet<OrderMedicament> OrderMedicaments { get; init; } = null!;
    public DbSet<OrderHistory> OrderHistory { get; init; } = null!;

    #endregion

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
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        optionsBuilder.UseSqlServer(_options.DbConnectionString);
    }
}