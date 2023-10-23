using AutoMapper;
using Core.Entities;
using MediatR;
using TaskManager.Contexts;
using TaskManager.ParserTasks.Contracts;
using TaskManager.RabbitMq;

namespace TaskManager.ParserTasks.Commands.CreateParserTask;

public class CreateParserTaskCommandHandler : IRequestHandler<CreateParserTaskCommand, Result<ParserTask>>
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateParserTaskCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IRabbitMqService _rabbitMqService;

    public CreateParserTaskCommandHandler(
        AppDbContext context,
        ILogger<CreateParserTaskCommandHandler> logger,
        IMapper mapper,
        IRabbitMqService rabbitMqService
    )
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;
        _rabbitMqService = rabbitMqService;
    }
    public async Task<Result<ParserTask>> Handle(CreateParserTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Добавить валидацию
            var taskType = _context.ParserTaskTypes.FirstOrDefault(x => x.Id == request.Type);
            if (taskType is null) return Result<ParserTask>.Failure("Тип задачи не найден");
            var taskStatus = _context.ParserTaskStatuses.FirstOrDefault(x => x.Key == "Created");
            if (taskStatus is null) return Result<ParserTask>.Failure("Статус задачи не найден");
            var parserTask = _mapper.Map<CreateParserTaskCommand, ParserTask>(request);
            parserTask.Type = taskType;
            parserTask.Status = taskStatus;
            await _context.AddAsync(parserTask, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _rabbitMqService.SendNewTaskMessage(parserTask);
            return Result<ParserTask>.Success(parserTask);
        }
        catch (Exception e)
        {
            var errorMessage = "Произошла ошибка при создании записи на прием";
            _logger.LogError(
                message: errorMessage,
                exception: e,
                args: new { request }
            );
            return Result<ParserTask>.Failure(errorMessage);
        }
    }
}