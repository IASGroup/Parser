using TaskManager.ParserTasks.Contracts.Core;

namespace TaskManager.Contexts;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();
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
}