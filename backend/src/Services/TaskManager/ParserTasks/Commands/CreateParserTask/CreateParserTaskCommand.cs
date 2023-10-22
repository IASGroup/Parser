using Core.Entities;
using MediatR;
using TaskManager.ParserTasks.Contracts;
using ParseWebsiteTagsOptions = TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation.ParseWebsiteTagsOptions;
using UrlOptions = TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation.UrlOptions;

namespace TaskManager.ParserTasks.Commands.CreateParserTask;

public class CreateParserTaskCommand : IRequest<Result<ParserTask>>
{
    public string Url { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public ParseWebsiteTagsOptions? WebsiteTagOptions { get; set; }
    public UrlOptions? UrlOptions { get; set; }
}