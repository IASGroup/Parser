using MediatR;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.DownloadTaskResults;

public class DownloadTaskResultsQuery : IRequest<Result<byte[]>>
{
	public Guid TaskId { get; set; }
}