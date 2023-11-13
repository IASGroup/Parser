using System.Text.Json;
using Collector.Contexts;
using Collector.Contracts;
using Collector.ParserTasks.Share;
using Collector.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Share.Tables;
using ParserTask = Share.RabbitMessages.ParserTaskAction.ParserTask;
using ParserTaskStatuses = Share.Contracts.ParserTaskStatuses;

namespace Collector.ParserTasks.Handlers.Api;

public class ParserTaskApiHandler : IParserTaskApiHandleService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<ParserTaskService> _logger;
	private readonly HttpClient _httpClient;
	private readonly IParserTaskUtilService _parserTaskUtilService;

	public ParserTaskApiHandler(
		IServiceProvider serviceProvider,
		ILogger<ParserTaskService> logger,
		IHttpClientFactory httpClientFactory,
		IParserTaskUtilService parserTaskUtilService
	)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
		_httpClient = httpClientFactory.CreateClient(HttpClientNames.Collector);
		_parserTaskUtilService = parserTaskUtilService;
	}

	public async Task Handle(ParserTask parserTaskInAction)
	{
		using var taskScope = _serviceProvider.CreateScope();
		var dbContext = taskScope.ServiceProvider.GetService<AppDbContext>();
		var rabbitMqService = taskScope.ServiceProvider.GetService<IRabbitMqService>();

		if (dbContext is null || rabbitMqService is null)
		{
			_logger.LogError(
				message: "Необходимый сервис не найден",
				args: new { dbContext, rabbitMqService }
			);
			return;
		}

		try
		{
			_logger.LogInformation($"Начало задачи парсинга: {parserTaskInAction.Id}");

			await dbContext.ParserTasks
				.Where(x => x.Id == parserTaskInAction.Id)
				.ExecuteUpdateAsync(x => x.SetProperty(
					y => y.StatusId, (int) ParserTaskStatuses.InProgress)
				);

			rabbitMqService.SendParserTaskCollectMessage(new()
			{
				ParserTaskId = parserTaskInAction.Id,
				ParserTaskStatusChangedMessage = new()
				{
					NewTaskStatus = (int) ParserTaskStatuses.InProgress
				}
			});
			foreach (var url in _parserTaskUtilService.GetParserTaskUrls(parserTaskInAction))
			{
				var request = new HttpRequestMessage
				{
					Method = _parserTaskUtilService.GetHttpMethodByName(parserTaskInAction.ParserTaskUrlOptions!.RequestMethod),
					RequestUri = new Uri(url)
				};
				var response = await _httpClient.SendAsync(request);
				var responseContent = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode)
				{
					rabbitMqService.SendParserTaskCollectMessage(new()
					{
						ParserTaskId = parserTaskInAction.Id,
						ParserTaskErrorMessage = new ParserTaskErrorMessage
						{
							ErrorMessage = JsonSerializer.Serialize(new
							{
								StatusCode = response.StatusCode,
								Content = responseContent
							}),
							Url = url
						}
					});
					dbContext.ParserTaskPartialResults.Add(new ParserTaskPartialResult()
					{
						StatusId = (int) ParserTaskPartialResultStatuses.Error,
						Url = url,
						Content = responseContent,
						ParserTaskId = parserTaskInAction.Id
					});
					await dbContext.SaveChangesAsync();
					return;
				}
				var newResult = new ParserTaskPartialResult
				{
					ParserTaskId = parserTaskInAction.Id,
					Url = url,
					StatusId = (int) ParserTaskPartialResultStatuses.Success,
					Content = responseContent
				};
				dbContext.ParserTaskPartialResults.Add(newResult);
				await dbContext.SaveChangesAsync();
			}
			await dbContext.ParserTasks
				.Where(x => x.Id == parserTaskInAction.Id)
				.ExecuteUpdateAsync(x => x.SetProperty(
					y => y.StatusId, (int) ParserTaskStatuses.Finished)
				);
			rabbitMqService.SendParserTaskCollectMessage(new()
			{
				ParserTaskId = parserTaskInAction.Id,
				ParserTaskStatusChangedMessage = new()
				{
					NewTaskStatus = (int) ParserTaskStatuses.Finished
				}
			});
			_logger.LogInformation($"Конец задачи парсинга: {parserTaskInAction.Id}");
		}
		catch (Exception e)
		{
			var errorMessage = $"Ошибка при выполнении задачи парсинга: {parserTaskInAction.Name}";

			await dbContext.ParserTasks
				.Where(x => x.Id == parserTaskInAction.Id)
				.ExecuteUpdateAsync(x => x.SetProperty(
					y => y.StatusId, (int) ParserTaskStatuses.Error)
				);

			rabbitMqService.SendParserTaskCollectMessage(new()
			{
				ParserTaskId = parserTaskInAction.Id,
				ParserTaskStatusChangedMessage = new()
				{
					NewTaskStatus = (int) ParserTaskStatuses.Error
				},
				ParserTaskErrorMessage = new()
				{
					ErrorMessage = errorMessage
				}
			});

			_logger.LogError(
				exception: e,
				message: errorMessage,
				args: new { parserTaskInAction }
			);
		}
	}
}