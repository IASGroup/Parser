﻿using System.Text.Json;
using Collector.Contexts;
using Collector.Contracts;
using Collector.ParserTasks.Handlers.Api;
using Collector.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Share.Tables;
using ParserTask = Share.RabbitMessages.ParserTaskAction.ParserTask;
using ParserTaskStatuses = Share.Contracts.ParserTaskStatuses;

namespace Collector.ParserTasks;

public class ParserTaskService : IParserTaskService
{
	private readonly ILogger<ParserTaskService> _logger;
	private readonly IParserTaskApiHandleService _parserTaskApiHandleService;

	public ParserTaskService(
		ILogger<ParserTaskService> logger,
		IParserTaskApiHandleService parserTaskApiHandleService
	)
	{
		_logger = logger;
		_parserTaskApiHandleService = parserTaskApiHandleService;
	}

	public async Task RunParserTaskHandler(ParserTask parserTaskInAction)
	{
		await (parserTaskInAction.TypeId switch
		{
			1 => _parserTaskApiHandleService.Handle(parserTaskInAction),
			_ => NotFoundTaskType(parserTaskInAction)
		});
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