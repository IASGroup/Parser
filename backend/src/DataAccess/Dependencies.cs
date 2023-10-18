using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        var srcPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName);
        var pathFromSrc = configuration.GetValue<string>("Database:Sqlite:FilePathFromSrc");
        if (string.IsNullOrEmpty(pathFromSrc) || srcPath is null) throw new Exception("DbFilePathNotFound");
        var absoluteDbPath = $"{srcPath}\\{pathFromSrc}";

        services.AddDbContext<AppDbContext>(x => x.UseSqlite(
            connectionString: $"Data Source={absoluteDbPath}"
        ));
    }
}