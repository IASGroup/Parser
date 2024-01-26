using MediatR;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.GetPartTaskResult;

public class GetPartTaskResultQuery : IRequest<Result<ParserTaskPartialResultDto>>
{
	public Guid TaskId { get; set; }
	public Guid ResultId { get; set; }
}