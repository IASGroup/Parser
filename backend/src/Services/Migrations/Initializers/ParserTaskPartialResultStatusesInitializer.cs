using Share.Tables;

namespace Migrations.Initializers;

public class ParserTaskPartialResultStatusesInitializer : IDbInitializer
{
    public void Initialize(AppDbContext context)
    {
        var statuses = context.ParserTaskPartialResultStatuses.ToList();
        if (!statuses.Exists(x => x.Id == 1))
        {
            context.ParserTaskPartialResultStatuses.Add(new ()
            {
                Id = 1,
                Key = "success",
                Description = "Успешный результат парсинга"
            });
        }
        if (!statuses.Exists(x => x.Id == 2))
        {
            context.ParserTaskPartialResultStatuses.Add(new ()
            {
                Id = 2,
                Key = "error",
                Description = "Результат с ошибкой"
            });
        }
    }
}