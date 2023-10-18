namespace DataAccess.Tables.ParserTask;

public class ParserTaskInitializer : IDbInitializer
{
    public void Initialize(AppDbContext context)
    {
        if (context.Database.EnsureCreated())
        {
            var newParserTaskType = new ParserTaskType()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "api",
                Description = "Задача парсинга api"
            };
            var newUrlOption = new UrlOptions()
            {
                Id = Guid.NewGuid().ToString(),
                RequestMethod = "GET"
            };
            var newParserTask = new ParserTask
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Тестовая задача парсинга",
                Type = newParserTaskType,
                Url = "https://jsonplaceholder.typicode.com/todos",
                WebsiteTagOptions = null,
                UrlOptions = newUrlOption
            };
            context.Add(newParserTaskType);
            context.Add(newUrlOption);
            context.Add(newParserTask);
            context.SaveChanges();
        }
    }
}