using System.Text.Json;
using Collector.Contexts;
using Collector.Contracts;
using Collector.ParserTasks.Share;
using Collector.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Share.Tables;
using System.Text.RegularExpressions;
using Collector.Tor;
using ParserTask = Share.RabbitMessages.ParserTaskAction.ParserTask;
using ParserTaskStatuses = Share.Contracts.ParserTaskStatuses;
using HtmlAgilityPack;

namespace Collector.ParserTasks.Handlers.Text;

public class ParserTaskTextHandler : IParserTaskTextHandleService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<ParserTaskService> _logger;
	private readonly HttpClient _httpClient;
	private readonly IParserTaskUtilService _parserTaskUtilService;

	public ParserTaskTextHandler(
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

	public async Task Handle(ParserTask parserTaskInAction, CancellationToken cancellationToken)
	{
		using var taskScope = _serviceProvider.CreateScope();
		var dbContext = taskScope.ServiceProvider.GetService<AppDbContext>();
		var rabbitMqService = taskScope.ServiceProvider.GetService<IRabbitMqService>();
		var torIntegrationService = taskScope.ServiceProvider.GetService<ITorIntegrationService>();

		if (dbContext is null || rabbitMqService is null || torIntegrationService is null)
		{
			_logger.LogError(
				message: "Необходимый сервис не найден"
			);
			return;
		}

		if (cancellationToken.IsCancellationRequested)
		{
			await PauseAsync(dbContext, parserTaskInAction, rabbitMqService);
			return;
		}

		try
		{
			_logger.LogInformation($"Начало задачи парсинга: {parserTaskInAction.Id}");

			await dbContext.ParserTasks
				.Where(x => x.Id == parserTaskInAction.Id)
				.ExecuteUpdateAsync(x => x.SetProperty(
					y => y.StatusId, (int) ParserTaskStatuses.InProgress),
					cancellationToken: cancellationToken
				);
			if (cancellationToken.IsCancellationRequested)
			{
				await PauseAsync(dbContext, parserTaskInAction, rabbitMqService);
				return;
			}
			rabbitMqService.SendParserTaskCollectMessage(new()
			{
				ParserTaskId = parserTaskInAction.Id,
				Type = ParserTaskCollectMessageTypes.StatusChanged,
				ParserTaskStatusChangedMessage = new()
				{
					NewTaskStatus = (int) ParserTaskStatuses.InProgress
				}
			});
			if (cancellationToken.IsCancellationRequested)
			{
				await PauseAsync(dbContext, parserTaskInAction, rabbitMqService);
				return;
			}

			var allUrls = _parserTaskUtilService.GetParserTaskUrls(parserTaskInAction).ToList();
			var handledUrls = await dbContext.ParserTaskPartialResults
				.Where(x => x.ParserTaskId == parserTaskInAction.Id && x.Url != null)
				.Select(x => x.Url!)
				.ToListAsync(CancellationToken.None);
			var needToHandleUrls = allUrls.Except(handledUrls).ToList();
			for (var index = 0; index < needToHandleUrls.Count; index++)
			{
				var url = needToHandleUrls[index];
				if (cancellationToken.IsCancellationRequested)
				{
					await PauseAsync(dbContext, parserTaskInAction, rabbitMqService);
					return;
				}

				string? responseContent;
				bool responseIsSuccess;
				
				if (parserTaskInAction.ParserTaskTorOptions is not null)
				{
					if ((index + 1) % parserTaskInAction.ParserTaskTorOptions.ChangeIpAddressAfterRequestsNumber == 0)
					{
						await torIntegrationService.ChangeIpAsync();
					}
					responseContent = await torIntegrationService.DownloadAsync(new TorDownloadRequestDto()
					{
						Url = url,
						MethodName = parserTaskInAction.ParserTaskUrlOptions!.RequestMethod
					});
					responseIsSuccess = responseContent is not null;
				}
				else
				{
					var request = new HttpRequestMessage
					{
						Method = _parserTaskUtilService.GetHttpMethodByName(parserTaskInAction.ParserTaskUrlOptions!.RequestMethod),
						RequestUri = new Uri(url)
					};
					var response = await _httpClient.SendAsync(request, cancellationToken);
					responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
					responseIsSuccess = response.IsSuccessStatusCode;
				}

				responseContent ??= "";
				
				if (!responseIsSuccess)
				{
					rabbitMqService.SendParserTaskCollectMessage(new()
					{
						ParserTaskId = parserTaskInAction.Id,
						Type = ParserTaskCollectMessageTypes.StatusChanged,
						ParserTaskErrorMessage = new ParserTaskErrorMessage
						{
							ErrorMessage = JsonSerializer.Serialize(new
							{
								Content = responseContent
							}),
							Url = url
						},
						ParserTaskStatusChangedMessage = new ParserTaskStatusChangedMessage
						{
							NewTaskStatus = (int) ParserTaskPartialResultStatuses.Error
						}
					});
					var failureResult = new ParserTaskPartialResult()
					{
						StatusId = (int) ParserTaskPartialResultStatuses.Error,
						Url = url,
						Content = responseContent,
						ParserTaskId = parserTaskInAction.Id
					};
					dbContext.ParserTaskPartialResults.Add(failureResult);
					await dbContext.SaveChangesAsync(cancellationToken);
					rabbitMqService.SendParserTaskCollectMessage(new()
					{
						ParserTaskId = parserTaskInAction.Id,
						Type = ParserTaskCollectMessageTypes.Progress,
						ParserTaskProgressMessage = new ParserTaskProgressMessage()
						{
							CompletedPartsNumber = (allUrls.Count - needToHandleUrls.Count) +
							                       (needToHandleUrls.IndexOf(url) + 1),
							CompletedPartUrl = url,
							NextPartUrl = needToHandleUrls.IndexOf(url) == needToHandleUrls.Count - 1
								? null
								: needToHandleUrls[needToHandleUrls.IndexOf(url) + 1],
							CompletedPartId = failureResult.Id,
							CompletedPartStatusId = failureResult.StatusId
						}
					});
					return;
				}

				string[] classesToRemove = {"ad", "advertisement", "promo"}; // Здесь список классов рекламы
				string[] tagsToRemove =
					{"script", "iframe", "img", "head", "footer", "nav", "a", "table"}; //список тегов для удаления
				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(responseContent);
				//Полностью удаляем мусорные теги
				foreach (var tag in tagsToRemove)
				{
					HtmlNodeCollection nodesToRemove = doc.DocumentNode.SelectNodes($"//{tag}");
					if (nodesToRemove != null)
					{
						foreach (HtmlNode node in nodesToRemove)
						{
							node.Remove();
						}
					}
				}

				//Удаляем теги по классам
				RemoveElementsByClass(doc.DocumentNode, classesToRemove);
				responseContent = doc.DocumentNode.OuterHtml;
				string pattern = "<.*?>";
				responseContent = Regex.Replace(responseContent, pattern, "");

				var newResult = new ParserTaskPartialResult
				{
					ParserTaskId = parserTaskInAction.Id,
					Url = url,
					StatusId = (int) ParserTaskPartialResultStatuses.Success,
					Content = responseContent
				};
				dbContext.ParserTaskPartialResults.Add(newResult);
				await dbContext.SaveChangesAsync(cancellationToken);
				rabbitMqService.SendParserTaskCollectMessage(new()
				{
					ParserTaskId = parserTaskInAction.Id,
					Type = ParserTaskCollectMessageTypes.Progress,
					ParserTaskProgressMessage = new ParserTaskProgressMessage()
					{
						CompletedPartsNumber = (allUrls.Count - needToHandleUrls.Count) +
						                       (needToHandleUrls.IndexOf(url) + 1),
						CompletedPartUrl = url,
						NextPartUrl = needToHandleUrls.IndexOf(url) == needToHandleUrls.Count - 1
							? null
							: needToHandleUrls[needToHandleUrls.IndexOf(url) + 1],
						CompletedPartId = newResult.Id,
						CompletedPartStatusId = newResult.StatusId
					}
				});
			}

			await dbContext.ParserTasks
				.Where(x => x.Id == parserTaskInAction.Id)
				.ExecuteUpdateAsync(x => x.SetProperty(
					y => y.StatusId, (int) ParserTaskStatuses.Finished),
					cancellationToken: cancellationToken
				);
			rabbitMqService.SendParserTaskCollectMessage(new()
			{
				ParserTaskId = parserTaskInAction.Id,
				Type = ParserTaskCollectMessageTypes.StatusChanged,
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
					y => y.StatusId, (int) ParserTaskStatuses.Error),
					cancellationToken: CancellationToken.None
				);

			rabbitMqService.SendParserTaskCollectMessage(new()
			{
				ParserTaskId = parserTaskInAction.Id,
				Type = ParserTaskCollectMessageTypes.StatusChanged,
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

	private async Task PauseAsync(
		AppDbContext dbContext,
		ParserTask parserTaskInAction,
		IRabbitMqService rabbitMqService
	)
	{
		await dbContext.ParserTasks
			.Where(x => x.Id == parserTaskInAction.Id)
			.ExecuteUpdateAsync(x => x.SetProperty(
				y => y.StatusId, (int) ParserTaskStatuses.Paused)
			);

		rabbitMqService.SendParserTaskCollectMessage(new()
		{
			ParserTaskId = parserTaskInAction.Id,
			Type = ParserTaskCollectMessageTypes.StatusChanged,
			ParserTaskStatusChangedMessage = new()
			{
				NewTaskStatus = (int) ParserTaskStatuses.Paused
			}
		});
	}
	static void RemoveElementsByClass(HtmlNode node, string[] classesToRemove)
	{
		if (node.NodeType == HtmlNodeType.Element && node.HasAttributes && node.Attributes.Contains("class"))
		{
			var classAttributeValue = node.GetAttributeValue("class", "");
			var nodeClasses = classAttributeValue.Split(' ');

			if (nodeClasses.Any(c => classesToRemove.Contains(c)))
			{
				node.Remove();
				return;
			}
		}

		for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
		{
			RemoveElementsByClass(node.ChildNodes[i], classesToRemove);
		}
	}


}