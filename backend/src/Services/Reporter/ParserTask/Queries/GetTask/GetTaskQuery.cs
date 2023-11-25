using MediatR;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.GetTask;

public class GetTaskQuery : IRequest<Result<ParserTaskDto>>
{
	public Guid ParserTaskId { get; set; }
}