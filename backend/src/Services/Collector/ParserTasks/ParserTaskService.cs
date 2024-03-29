﻿using Collector.ParserTasks.Handlers.Api;
using Collector.ParserTasks.Handlers.Text;
using Collector.ParserTasks.Handlers.Tags;
using ParserTask = Share.RabbitMessages.ParserTaskAction.ParserTask;

namespace Collector.ParserTasks;

public class ParserTaskService : IParserTaskService
{
	private readonly ILogger<ParserTaskService> _logger;
	private readonly IParserTaskApiHandleService _parserTaskApiHandleService;
	private readonly IParserTaskTextHandleService _parserTaskTextHandleService;
	private readonly IParserTaskTagsHandleService _parserTaskTagsHandleService;
	private readonly Dictionary<Guid, CancellationTokenSource> _cancellationTokenSources;

	public ParserTaskService(
		ILogger<ParserTaskService> logger,
		IParserTaskApiHandleService parserTaskApiHandleService,
		IParserTaskTextHandleService parserTaskTextHandleService,
		IParserTaskTagsHandleService parserTaskTagsHandleService
	)
	{
		_logger = logger;
		_parserTaskApiHandleService = parserTaskApiHandleService;
		_parserTaskTextHandleService = parserTaskTextHandleService;
		_parserTaskTagsHandleService = parserTaskTagsHandleService;
		_cancellationTokenSources = new Dictionary<Guid, CancellationTokenSource>();
	}

	public async Task HandleRunParserTaskMessageAsync(ParserTask parserTaskInAction)
	{
		var tokenSource = new CancellationTokenSource();
		_cancellationTokenSources.Add(parserTaskInAction.Id, tokenSource);
		await (parserTaskInAction.TypeId switch
		{
			1 => _parserTaskApiHandleService.Handle(parserTaskInAction, tokenSource.Token),
			2 => _parserTaskTextHandleService.Handle(parserTaskInAction, tokenSource.Token),
			3 => _parserTaskTagsHandleService.Handle(parserTaskInAction, tokenSource.Token),
			_ => NotFoundTaskType(parserTaskInAction)
		});
		_cancellationTokenSources.Remove(parserTaskInAction.Id);
	}

	public Task HandleStopParserTaskMessageAsync(ParserTask parserTaskInAction)
	{
		if (!_cancellationTokenSources.TryGetValue(parserTaskInAction.Id, out var tokenSource))
		{
			return Task.CompletedTask;
		}
		tokenSource!.Cancel();
		_cancellationTokenSources.Remove(parserTaskInAction.Id);
		_logger.LogInformation($"Задача остановлена: {parserTaskInAction.Id}");
		return Task.CompletedTask;
	}

	private Task NotFoundTaskType(ParserTask parserTaskInAction)
	{
		const string errorMessage = "Тип задачи парсинга не найден";
		_logger.LogError(
			message: errorMessage,
			args: new { parserTaskInAction }
		);
		return Task.CompletedTask;
	}
}