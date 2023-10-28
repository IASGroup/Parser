using System.Reflection;
using Collector.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Share.Tables;

namespace Collector.Contexts;

public class AppDbContext : DbContext
{
    private readonly IOptionsSnapshot<DbOptions> _dbOptions;

    public AppDbContext(DbContextOptions<AppDbContext> options, IOptionsSnapshot<DbOptions> dbOptions) : base(options)
    {
        _dbOptions = dbOptions;
    }

    public DbSet<ParserTaskResult> ParserTaskResults { get; set; }
    public DbSet<ParserTaskStatuses> ParserTaskStatuses { get; set; }
    public DbSet<ParserTask> ParserTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var connectionString = $"Server={_dbOptions.Value.Host};" +
            $"Port={_dbOptions.Value.Port};" +
            $"Database={_dbOptions.Value.DbName};" +
            $"User Id={_dbOptions.Value.UserName};" +
            $"Password={_dbOptions.Value.UserPassword};";
        optionsBuilder.UseNpgsql(connectionString);
    }
}