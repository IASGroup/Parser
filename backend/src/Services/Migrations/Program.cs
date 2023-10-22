using Core.Entities;
using Microsoft.Extensions.Configuration;
using Migrations;
using Migrations.Options;

var inDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
Console.WriteLine(inDocker);
var dicrectory = inDocker ?
    Directory.GetCurrentDirectory()
    : Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
var configPath = inDocker ?
    $"{dicrectory}/config-docker.json"
    : $"{dicrectory}/config.json";
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(configPath, optional: false);
IConfiguration config = builder.Build();
var dbOptions = config.GetSection(DbOptions.Name).Get<DbOptions>();

using var context = new AppDbContext(dbOptions!);
if (context.Database.EnsureCreated())
{
    var types = context.ParserTaskTypes.ToList();
    if (!types.Exists(x => x.Id == 1))
    {
        context.ParserTaskTypes.Add(new ParserTaskType
        {
            Id = 1,
            Name = "api",
            Description = "Задача парсинга API"
        });
    }

    if (!types.Exists(x => x.Id == 2))
    {
        context.ParserTaskTypes.Add(new ParserTaskType
        {
            Id = 2,
            Name = "webText",
            Description = "Задача парсинга текста сайта"
        });
    }

    if (!types.Exists(x => x.Id == 3))
    {
        context.ParserTaskTypes.Add(new ParserTaskType
        {
            Id = 3,
            Name = "webTags",
            Description = "Задача парсинга тегов сайта"
        });
    }

    context.SaveChanges();
}