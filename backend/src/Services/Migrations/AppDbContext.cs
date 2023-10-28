using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Migrations.Options;
using Share.Tables;

namespace Migrations;

public class AppDbContext : DbContext
{
    private readonly DbOptions _dbOptions;

    public AppDbContext(DbOptions dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public DbSet<ParserTask> ParserTasks { get; set; }
    public DbSet<ParserTaskType> ParserTaskTypes { get; set; }
    public DbSet<ParserTaskResult> ParserTaskResults { get; set; }
    public DbSet<ParserTaskStatuses> ParserTaskStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var connectionString = $"Server={_dbOptions.Host};" +
                               $"Port={_dbOptions.Port};" +
                               $"Database={_dbOptions.DbName};" +
                               $"User Id={_dbOptions.UserName};" +
                               $"Password={_dbOptions.UserPassword};";
        optionsBuilder.UseNpgsql(connectionString);
    }
}