using Microsoft.Extensions.Configuration;
using Migrations;
using Migrations.Options;
using Share.Tables;

var inDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
Console.WriteLine(inDocker);
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

    var statuses = context.ParserTaskStatuses.ToList();
    if (!statuses.Exists(x => x.Id == 1))
    {
        context.ParserTaskStatuses.Add(new ParserTaskStatuses
        {
            Id = 1,
            Key = "Created",
            Description = "Задача парсинга создана"
        });
    }
    if (!statuses.Exists(x => x.Id == 2))
    {
        context.ParserTaskStatuses.Add(new ParserTaskStatuses
        {
            Id = 2,
            Key = "InProgress",
            Description = "Задача парсинга в работе"
        });
    }
    if (!statuses.Exists(x => x.Id == 3))
    {
        context.ParserTaskStatuses.Add(new ParserTaskStatuses
        {
            Id = 3,
            Key = "Paused",
            Description = "Задача парсинга остановлена"
        });
    }
    if (!statuses.Exists(x => x.Id == 4))
    {
        context.ParserTaskStatuses.Add(new ParserTaskStatuses
        {
            Id = 4,
            Key = "Error",
            Description = "При выполнении задачи возникла ошибка"
        });
    }
    if (!statuses.Exists(x => x.Id == 5))
    {
        context.ParserTaskStatuses.Add(new ParserTaskStatuses
        {
            Id = 5,
            Key = "Finished",
            Description = "Задача парсинга завершена"
        });
    }

    context.SaveChanges();
}