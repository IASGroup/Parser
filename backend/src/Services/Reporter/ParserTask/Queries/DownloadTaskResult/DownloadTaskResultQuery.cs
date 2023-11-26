using MediatR;
using Reporter.ParserTask.Contracts;

namespace Reporter.ParserTask.Queries.DownloadTaskResult;

public class DownloadTaskResultQuery : IRequest<Result<byte[]>>
{
	public Guid TaskId { get; set; }
	public Guid ResultId { get; set; }
}