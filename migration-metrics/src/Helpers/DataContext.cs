namespace MigrationMetrics.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using MigrationMetrics.Entities;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"DataSource={Configuration["DATABASE_LOCATION"]};");
    }

    public DbSet<MonthlyCountStat> MonthlyCountStats { get; set; }
}