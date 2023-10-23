using Core.Entities;
using MediatR;
using TaskManager.ParserTasks.Contracts;
using ParserTaskUrlOptions = TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation.ParserTaskUrlOptions;
using ParserTaskWebsiteTagsOptions = TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation.ParserTaskWebsiteTagsOptions;

namespace TaskManager.ParserTasks.Commands.CreateParserTask;

public class CreateParserTaskCommand : IRequest<Result<ParserTask>>
{
    public string Url { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public ParserTaskWebsiteTagsOptions? ParserTaskWebsiteTagsOptions { get; set; }
    public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }
}