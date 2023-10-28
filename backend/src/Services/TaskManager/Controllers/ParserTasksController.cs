using MediatR;
using Microsoft.AspNetCore.Mvc;
using Share.Tables;
using TaskManager.ParserTasks.Commands.CreateParserTask;
using TaskManager.ParserTasks.Commands.CreateParserTask.Request;
using TaskManager.ParserTasks.Commands.CreateParserTask.Response;

namespace TaskManager.Controllers;
[ApiController]
[Route("api/parser-tasks")]
public class ParserTasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParserTasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateParserTaskResponseDto>> CreateParserTaskAsync([FromBody] CreateParserTaskCommand createCommand)
    {
        var createCommandResult = await _mediator.Send(createCommand);
        return createCommandResult.IsSuccess ?
            Ok(createCommandResult.Value)
            : BadRequest(createCommandResult.ErrorMessage);
    }
}