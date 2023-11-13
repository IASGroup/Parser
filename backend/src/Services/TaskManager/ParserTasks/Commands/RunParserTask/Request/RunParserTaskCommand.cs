using MediatR;
using TaskManager.ParserTasks.Commands.RunParserTask.Response;
using TaskManager.ParserTasks.Contracts;

namespace TaskManager.ParserTasks.Commands.RunParserTask.Request;

public class RunParserTaskCommand : IRequest<Result<RunParserTaskResponseDto>>
{
	public Guid TaskId { get; set; }
}