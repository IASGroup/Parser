using MediatR;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.GetTaskResults;

public class GetTaskResultsQuery : IRequest<Result<IEnumerable<ParserTaskResult>>>
{
	public Guid ParserTaskId { get; set; }
}