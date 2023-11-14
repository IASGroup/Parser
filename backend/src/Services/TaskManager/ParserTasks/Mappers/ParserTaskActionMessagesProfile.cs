using AutoMapper;
using Action = Share.RabbitMessages.ParserTaskAction;
using Tables = Share.Tables;

namespace TaskManager.ParserTasks.Mappers;

public class CreateParserTaskMessagesProfile : Profile
{
	public CreateParserTaskMessagesProfile()
	{
		CreateMap<Tables.ParserTaskWebsiteTagsOptions, Action.ParserTaskWebsiteTagsOptions>();
		CreateMap<Tables.ParserTaskWebsiteTag, Action.ParserTaskWebsiteTag>();
		CreateMap<Tables.FindOptions, Action.FindOptions>();
		CreateMap<Tables.TagAttribute, Action.TagAttribute>();
		CreateMap<Tables.ParserTaskUrlOptions, Action.ParserTaskUrlOptions>();
		CreateMap<Tables.PostMethodOptions, Action.PostMethodOptions>();
		CreateMap<Tables.Query, Action.Query>();
		CreateMap<Tables.Path, Action.Path>();
		CreateMap<Tables.Header, Action.Header>();
		CreateMap<Tables.ValueOptions, Action.ValueOptions>();
		CreateMap<Tables.ValueItem, Action.ValueItem>();
		CreateMap<Tables.Range, Action.Range>();
		CreateMap<Tables.ParserTask, Action.ParserTask>();
	}
}