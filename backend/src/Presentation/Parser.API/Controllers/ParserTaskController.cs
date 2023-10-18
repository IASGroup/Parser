using AutoMapper;
using BussinessLogic.Contracts.ParserTask;
using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace Parser.API.Controllers;

[ApiController]
[Route("api/parsertasks")]
public class ParserTaskController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IMapper mapper;

    public ParserTaskController(AppDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<ParserTaskDto>> GetAllTasksAsync()
    {
        var models = dbContext.ParserTasks.ToList();
        return mapper.Map<IEnumerable<ParserTaskDto>>(models);
    }
}