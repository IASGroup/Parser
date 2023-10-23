using AutoMapper;
using Core.Entities;
using TaskManager.ParserTasks.Commands.CreateParserTask;
using Create = TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation;
using Path = Core.Entities.Path;
using Range = Core.Entities.Range;

namespace TaskManager.ParserTasks.Mappers;

public class ParserTaskProfile : Profile
{
    public ParserTaskProfile()
    {
        CreateMap<Create.ParserTaskWebsiteTagsOptions, ParserTaskWebsiteTagsOptions>();
        CreateMap<Create.ParserTaskWebsiteTag, ParserTaskWebsiteTag>();
        CreateMap<Create.FindOptions, FindOptions>();
        CreateMap<Create.TagAttribute, TagAttribute>();
        CreateMap<Create.ParserTaskUrlOptions, ParserTaskUrlOptions>();
        CreateMap<Create.PostMethodOptions, PostMethodOptions>();
        CreateMap<Create.Query, Query>();
        CreateMap<Create.Path, Path>();
        CreateMap<Create.Header, Header>();
        CreateMap<Create.ValueOptions, ValueOptions>();
        CreateMap<Create.ValueItem, ValueItem>();
        CreateMap<Create.Range, Range>();
        CreateMap<CreateParserTaskCommand, ParserTask>()
            .ForMember(x => x.Type, y => y.Ignore());
    }
}