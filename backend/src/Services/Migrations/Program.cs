using Microsoft.Extensions.Configuration;
using Migrations;
using Migrations.Initializers;
using Migrations.Options;

var inDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

var dicrectory = inDocker ?
    Directory.GetCurrentDirectory()
    : Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;

var configPath = inDocker ?
    $"{dicrectory}/config-docker.json"
    : $"{dicrectory}/config.json";

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(configPath, optional: false);

IConfiguration config = builder.Build();
var dbOptions = config.GetSection(DbOptions.Name).Get<DbOptions>();

using var context = new AppDbContext(dbOptions!);

DbInitializer.Initialize(context);

Console.WriteLine("Миграции применены");