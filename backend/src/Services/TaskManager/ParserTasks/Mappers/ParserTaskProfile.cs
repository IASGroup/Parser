using AutoMapper;
using TaskManager.ParserTasks.Commands.CreateParserTask;
using Create = TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation;
using Core = TaskManager.ParserTasks.Contracts.Core;

namespace TaskManager.ParserTasks.Mappers;

public class ParserTaskProfile : Profile
{
    public ParserTaskProfile()
    {
        CreateMap<Create.ParseWebsiteTagsOptions, Core.ParseWebsiteTagsOptions>();
        CreateMap<Create.Tag, Core.Tag>();
        CreateMap<Create.FindOptions, Core.FindOptions>();
        CreateMap<Create.TagAttribute, Core.TagAttribute>();
        CreateMap<Create.UrlOptions, Core.UrlOptions>();
        CreateMap<Create.PostMethodOptions, Core.PostMethodOptions>();
        CreateMap<Create.Query, Core.Query>();
        CreateMap<Create.Path, Core.Path>();
        CreateMap<Create.Header, Core.Header>();
        CreateMap<Create.ValueOptions, Core.ValueOptions>();
        CreateMap<Create.ValueItem, Core.ValueItem>();
        CreateMap<Create.Range, Core.Range>();
        CreateMap<CreateParserTaskCommand, Core.ParserTask>()
            .ForMember(x => x.Type, y => y.Ignore());
    }
}