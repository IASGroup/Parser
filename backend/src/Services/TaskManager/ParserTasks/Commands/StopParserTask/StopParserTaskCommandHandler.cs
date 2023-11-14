using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using TaskManager.Contexts;
using TaskManager.ParserTasks.Commands.StopParserTask.Request;
using TaskManager.ParserTasks.Commands.StopParserTask.Response;
using TaskManager.ParserTasks.Contracts;
using TaskManager.RabbitMq;
using Action = Share.RabbitMessages.ParserTaskAction;

namespace TaskManager.ParserTasks.Commands.StopParserTask;

public class StopParserTaskCommandHandler : IRequestHandler<StopParserTaskCommand, Result<StopParserTaskResponseDto>>
{
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;
	private readonly IRabbitMqService _rabbitMqService;

	public StopParserTaskCommandHandler(
		AppDbContext context,
		IMapper mapper,
		IRabbitMqService rabbitMqService
	)
	{
		_context = context;
		_mapper = mapper;
		_rabbitMqService = rabbitMqService;
	}

	public async Task<Result<StopParserTaskResponseDto>> Handle(StopParserTaskCommand request, CancellationToken cancellationToken)
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
			return Result<StopParserTaskResponseDto>.Failure(errorMessage);
		}

		if (taskModel.StatusId is not (int) ParserTaskStatuses.InProgress)
		{
			const string errorMessage = "Задача не запущена";
			return Result<StopParserTaskResponseDto>.Failure(errorMessage);
		}

		var parserTaskInMessage = _mapper.Map<Action.ParserTask>(taskModel);
		_rabbitMqService.SendTaskActionMessage(new Action.ParserTaskActionMessage()
		{
			ParserTask = parserTaskInMessage,
			ParserTaskAction = Action.ParserTaskActions.Pause
		});
		return Result<StopParserTaskResponseDto>.Success(new StopParserTaskResponseDto
		{
			ParserTaskId = taskModel.Id
		});
	}
}