using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporter.ParserTask.Queries;

namespace Reporter.Controllers;

[ApiController]
[Route("api/parser-tasks")]
public class ParserTaskController : ControllerBase
{
	private readonly IMediator _mediator;

	public ParserTaskController(IMediator mediator)
	{
		_mediator = mediator;
	}

	[HttpGet("{taskId}/results")]
	public async Task<IActionResult> GetTaskResultsAsync([FromRoute] Guid taskId)
	{
		var query = new GetTaskResultsQuery() { TaskId = taskId };
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? File(result.Value!, "text/plain", "result.txt")
			: BadRequest(result.ErrorMessage);
	}
	[HttpGet("{taskId}/results/{resultId}")]
	public async Task<IActionResult> GetPartTaskResultAsync([FromRoute] Guid taskId,[FromRoute] Guid resultId)
	{
		var query = new GetPartTaskResultQuery() { TaskId = taskId,ResultId = resultId};
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? Ok(result.Value)
			: BadRequest(result.ErrorMessage);
	}
}