using MediatR;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.GetTaskResults;

public class GetTaskResultsQuery : IRequest<Result<byte[]>>
{
	public Guid TaskId { get; set; }
}