using Share.Tables;

namespace Migrations.Initializers;

public class ParserTaskStatusesInitializer : IDbInitializer
{
	public void Initialize(AppDbContext context)
	{
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
	}
}