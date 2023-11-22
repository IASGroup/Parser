using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries;

public class GetTaskResultsQuery : IRequest<Result<byte[]>>
{
	public Guid TaskId { get; set; }
}