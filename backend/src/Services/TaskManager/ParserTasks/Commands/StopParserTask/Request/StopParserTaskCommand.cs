using MediatR;
using TaskManager.ParserTasks.Commands.StopParserTask.Response;
using TaskManager.ParserTasks.Contracts;

namespace TaskManager.ParserTasks.Commands.StopParserTask.Request;

public class StopParserTaskCommand : IRequest<Result<StopParserTaskResponseDto>>
{
	public Guid TaskId { get; set; }
}