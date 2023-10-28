using AutoMapper;
using Tables = Share.Tables;
using Messages = Share.RabbitMessages;
namespace TaskManager.ParserTasks.Commands.CreateParserTask.Mappers;

public class CreateParserTaskMessagesProfile : Profile
{
    public CreateParserTaskMessagesProfile()
    {
        CreateMap<Tables.ParserTaskWebsiteTagsOptions, Messages.ParserTaskWebsiteTagsOptions>();
        CreateMap<Tables.ParserTaskWebsiteTag, Messages.ParserTaskWebsiteTag>();
        CreateMap<Tables.FindOptions, Messages.FindOptions>();
        CreateMap<Tables.TagAttribute, Messages.TagAttribute>();
        CreateMap<Tables.ParserTaskUrlOptions, Messages.ParserTaskUrlOptions>();
        CreateMap<Tables.PostMethodOptions, Messages.PostMethodOptions>();
        CreateMap<Tables.Query, Messages.Query>();
        CreateMap<Tables.Path, Messages.Path>();
        CreateMap<Tables.Header, Messages.Header>();
        CreateMap<Tables.ValueOptions, Messages.ValueOptions>();
        CreateMap<Tables.ValueItem, Messages.ValueItem>();
        CreateMap<Tables.Range, Messages.Range>();
        CreateMap<Tables.ParserTask, Messages.NewParserTaskMessage>();
    }
}