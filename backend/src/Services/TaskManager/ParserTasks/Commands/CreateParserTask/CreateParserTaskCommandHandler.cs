using AutoMapper;
using MediatR;
using TaskManager.Contexts;
using TaskManager.ParserTasks.Commands.CreateParserTask.Request;
using TaskManager.ParserTasks.Commands.CreateParserTask.Response;
using TaskManager.ParserTasks.Contracts;
using TaskManager.ParserTasks.Share;
using TaskManager.RabbitMq;
using ParserTask = Share.Tables.ParserTask;

namespace TaskManager.ParserTasks.Commands.CreateParserTask;

public class CreateParserTaskCommandHandler : IRequestHandler<CreateParserTaskCommand, Result<CreateParserTaskResponseDto>>
{
	private readonly AppDbContext _context;
	private readonly ILogger<CreateParserTaskCommandHandler> _logger;
	private readonly IMapper _mapper;
	private readonly IRabbitMqService _rabbitMqService;
	private readonly IParserTaskUtilService _parserTaskUtilService;

	public CreateParserTaskCommandHandler(
		AppDbContext context,
		ILogger<CreateParserTaskCommandHandler> logger,
		IMapper mapper,
		IRabbitMqService rabbitMqService,
		IParserTaskUtilService parserTaskUtilService
	)
	{
		_context = context;
		_logger = logger;
		_mapper = mapper;
		_rabbitMqService = rabbitMqService;
		_parserTaskUtilService = parserTaskUtilService;
	}
	public async Task<Result<CreateParserTaskResponseDto>> Handle(CreateParserTaskCommand request, CancellationToken cancellationToken)
	{
		try
		{
			var taskType = _context.ParserTaskTypes.FirstOrDefault(x => x.Id == request.TypeId);
			if (taskType is null) return Result<CreateParserTaskResponseDto>.Failure("Тип задачи не найден");
			var taskStatus = _context.ParserTaskStatuses.FirstOrDefault(x => x.Key == "Created");
			if (taskStatus is null) return Result<CreateParserTaskResponseDto>.Failure("Статус задачи не найден");
			var parserTask = _mapper.Map<CreateParserTaskCommand, ParserTask>(request);
			parserTask.Type = taskType;
			parserTask.Status = taskStatus;
			await _context.AddAsync(parserTask, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			var allPartsNumber = _parserTaskUtilService.GetParserTaskUrls(parserTask).Count();
			var response = _mapper.Map<ParserTask, CreateParserTaskResponseDto>(parserTask);
			response.AllPartsNumber = allPartsNumber;
			return Result<CreateParserTaskResponseDto>.Success(response);
		}
		catch (Exception e)
		{
			var errorMessage = "Произошла ошибка при создании записи на прием";
			_logger.LogError(
				message: errorMessage,
				exception: e,
				args: new { request }
			);
			return Result<CreateParserTaskResponseDto>.Failure(errorMessage);
		}
	}
}