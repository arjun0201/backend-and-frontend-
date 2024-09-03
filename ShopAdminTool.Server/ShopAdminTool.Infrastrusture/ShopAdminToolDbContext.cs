using Microsoft.EntityFrameworkCore;
using ShopAdminTool.Core;
using Microsoft.Extensions.Options;

namespace ShopAdminTool.Infrastrusture;

public class ShopAdminToolDbContext : DbContext
{
    protected readonly DatabaseSettings _dbSettings;

    public ShopAdminToolDbContext(DbContextOptions<ShopAdminToolDbContext> options, IOptions<DatabaseSettings> dbSettings)
            : base(options)
    {
        _dbSettings = dbSettings.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        UseDatabase(options, _dbSettings.DBProvider, _dbSettings.ConnectionString);
    }

    private void UseDatabase(DbContextOptionsBuilder options, string dbProvider, string connectionString)
    {
        switch(dbProvider)
        {
            case DbProviderKeys.SqlServer:
                options.UseSqlServer(connectionString);
                break;
            case DbProviderKeys.SqLite:
                options.UseSqlite(connectionString);
                break;
            default: throw new Exception($"Storage Provider {dbProvider} is not supported.");
        };
    }

    public virtual required DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(f => f.Id)
            .IsUnique();
    }
}
