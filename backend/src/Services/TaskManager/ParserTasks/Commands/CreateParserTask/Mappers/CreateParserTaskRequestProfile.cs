using AutoMapper;
using Share.Tables;
using Path = Share.Tables.Path;
using Range = Share.Tables.Range;

namespace TaskManager.ParserTasks.Commands.CreateParserTask.Mappers;

public class CreateParserTaskRequestProfile : Profile
{
	public CreateParserTaskRequestProfile()
	{
		CreateMap<Request.ParserTaskWebsiteTagsOptions, ParserTaskWebsiteTagsOptions>();
		CreateMap<Request.ParserTaskWebsiteTag, ParserTaskWebsiteTag>();
		CreateMap<Request.FindOptions, FindOptions>();
		CreateMap<Request.TagAttribute, TagAttribute>();
		CreateMap<Request.ParserTaskUrlOptions, ParserTaskUrlOptions>()
			.ForMember(x => x.RequestMethod, y => y.MapFrom(z => z.RequestMethod))
			.ForMember(x => x.Id, y => y.Ignore())
			.ForMember(x => x.PostMethodOptionsId, y => y.Ignore());
		CreateMap<Request.PostMethodOptions, PostMethodOptions>();
		CreateMap<Request.Query, Query>();
		CreateMap<Request.Path, Path>();
		CreateMap<Request.Header, Header>();
		CreateMap<Request.ValueOptions, ValueOptions>();
		CreateMap<Request.ValueItem, ValueItem>();
		CreateMap<Request.Range, Range>();
		CreateMap<Request.CreateParserTaskCommand, ParserTask>()
			.ForMember(x => x.Id, y => y.Ignore())
			.ForMember(x => x.StatusId, y => y.MapFrom(z => 0))
			.ForMember(x => x.ParserTaskUrlOptionsId, y => y.Ignore())
			.ForMember(x => x.ParserTaskWebsiteTagsOptionsId, y => y.Ignore());
	}
}