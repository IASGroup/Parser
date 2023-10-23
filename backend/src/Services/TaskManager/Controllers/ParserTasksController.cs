using Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.ParserTasks.Commands.CreateParserTask;

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
    public async Task<ActionResult<ParserTask>> CreateParserTaskAsync([FromBody] CreateParserTaskCommand createCommand)
    {
        var createCommandResult = await _mediator.Send(createCommand);
        return createCommandResult.IsSuccess ?
            Ok(createCommandResult.Value)
            : BadRequest(createCommandResult.ErrorMessage);
    }
}