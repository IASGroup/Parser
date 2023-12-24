using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries;

public class GetPartTaskResultQuery : IRequest<Result<ParserTaskPartialResultDto>>
{
	public Guid TaskId { get; set; }
	public Guid ResultId { get; set; }
}