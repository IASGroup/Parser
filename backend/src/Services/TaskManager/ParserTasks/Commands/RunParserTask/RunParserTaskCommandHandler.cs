using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Action = Share.RabbitMessages.ParserTaskAction;
using TaskManager.Contexts;
using TaskManager.ParserTasks.Commands.RunParserTask.Request;
using TaskManager.ParserTasks.Commands.RunParserTask.Response;
using TaskManager.ParserTasks.Contracts;
using TaskManager.RabbitMq;

namespace TaskManager.ParserTasks.Commands.RunParserTask;

public class RunParserTaskCommandHandler : IRequestHandler<RunParserTaskCommand, Result<RunParserTaskResponseDto>>
{
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;
	private readonly IRabbitMqService _rabbitMqService;

	public RunParserTaskCommandHandler(
		AppDbContext context,
		IMapper mapper,
		IRabbitMqService rabbitMqService
	)
	{
		_context = context;
		_mapper = mapper;
		_rabbitMqService = rabbitMqService;
	}

	public async Task<Result<RunParserTaskResponseDto>> Handle(
		RunParserTaskCommand request,
		CancellationToken cancellationToken
	)
	{
		var taskModel = await _context.ParserTasks
			.Include(x => x.ParserTaskUrlOptions!.PostMethodOptions)
			.Include(x => x.ParserTaskUrlOptions!.Queries)!
			.ThenInclude(x => x.ValueOptions!.Values)
			.Include(x => x.ParserTaskUrlOptions!.Queries)!
			.ThenInclude(x => x.ValueOptions!.Range)
			.Include(x => x.ParserTaskUrlOptions!.Paths)!
			.ThenInclude(x => x.ValueOptions!.Values)
			.Include(x => x.ParserTaskUrlOptions!.Paths)!
			.ThenInclude(x => x.ValueOptions!.Range)
			.Include(x => x.ParserTaskUrlOptions!.Headers)
			.Include(x => x.ParserTaskWebsiteTagsOptions!.ParserTaskWebsiteTags)!
				.ThenInclude(x => x.FindOptions!.Attributes)
		.FirstOrDefaultAsync(
			predicate: x => x.Id == request.TaskId,
			cancellationToken: cancellationToken
		);
		if (taskModel is null)
		{
			const string errorMessage = "Задача не нейдена";
			return Result<RunParserTaskResponseDto>.Failure(errorMessage);
		}

		if (taskModel.StatusId is (int) ParserTaskStatuses.InProgress or (int) ParserTaskStatuses.Finished)
		{
			const string errorMessage = "Задача уже запущена или завершена";
			return Result<RunParserTaskResponseDto>.Failure(errorMessage);
		}

		var parserTaskInMessage = _mapper.Map<Action.ParserTask>(taskModel);
		_rabbitMqService.SendTaskActionMessage(new Action.ParserTaskActionMessage()
		{
			ParserTask = parserTaskInMessage,
			ParserTaskAction = Action.ParserTaskActions.Run
		});
		return Result<RunParserTaskResponseDto>.Success(new RunParserTaskResponseDto()
		{
			ParserTaskId = taskModel.Id
		});
	}
}