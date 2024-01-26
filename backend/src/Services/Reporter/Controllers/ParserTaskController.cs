using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reporter.ParserTask.Queries.DownloadTaskResult;
using Reporter.ParserTask.Queries.DownloadTaskResults;
using Reporter.ParserTask.Queries.GetPartTaskResult;
using Reporter.ParserTask.Queries.GetTask;
using Reporter.ParserTask.Queries.GetTaskResults;
using Reporter.ParserTask.Queries.GetTasks;

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

	[HttpGet("{taskId:guid}/results/download")]
	public async Task<IActionResult> DownloadTaskResultsAsync([FromRoute] Guid taskId)
	{
		var query = new DownloadTaskResultsQuery() { TaskId = taskId };
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? File(result.Value!, "text/plain", "result.txt")
			: BadRequest(result.ErrorMessage);
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<ParserTaskListItemDto>>> GetAllTasksAsync()
	{
		var query = new GetTasksQuery();
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? Ok(result.Value)
			: BadRequest(result.ErrorMessage);
	}

	[HttpGet("{taskId:guid}/results")]
	public async Task<ActionResult<IEnumerable<ParserTaskResult>>> GetTaskResultsAsync([FromRoute] Guid taskId)
	{
		var query = new GetTaskResultsQuery { ParserTaskId = taskId };
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? Ok(result.Value)
			: BadRequest(result.ErrorMessage);
	}

	[HttpGet("{taskId:guid}")]
	public async Task<ActionResult<IEnumerable<ParserTaskDto>>> GetTaskAsync([FromRoute] Guid taskId)
	{
		var query = new GetTaskQuery { ParserTaskId = taskId };
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? Ok(result.Value)
			: BadRequest(result.ErrorMessage);
	}

	[HttpGet("{taskId:guid}/results/{resultId}/download")]
	public async Task<IActionResult> DownloadTaskPartialResultAsync([FromRoute] DownloadTaskResultQuery query)
	{
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? File(result.Value!, "text/plain", "result.txt")
			: BadRequest(result.ErrorMessage);
	}
	
	[HttpGet("{taskId:guid}/results/{resultId}")]
	public async Task<ActionResult<ParserTaskPartialResultDto>> GetPartTaskResultAsync(
		[FromRoute] Guid taskId,
		[FromRoute] Guid resultId
	)
	{
		var query = new GetPartTaskResultQuery() { TaskId = taskId,ResultId = resultId};
		var result = await _mediator.Send(query);
		return result.IsSuccess
			? Ok(result.Value)
			: BadRequest(result.ErrorMessage);
	}
}