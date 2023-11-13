using AutoMapper;
using Request = TaskManager.ParserTasks.Commands.CreateParserTask.Request;

namespace TaskManager.ParserTasks.Commands.CreateParserTask.Mappers;

public class CreateParserTaskRequestProfile : Profile
{
	public CreateParserTaskRequestProfile()
	{
		CreateMap<Request.ParserTaskWebsiteTagsOptions, Share.Tables.ParserTaskWebsiteTagsOptions>();
		CreateMap<Request.ParserTaskWebsiteTag, Share.Tables.ParserTaskWebsiteTag>();
		CreateMap<Request.FindOptions, Share.Tables.FindOptions>();
		CreateMap<Request.TagAttribute, Share.Tables.TagAttribute>();
		CreateMap<Request.ParserTaskUrlOptions, Share.Tables.ParserTaskUrlOptions>()
			.ForMember(x => x.RequestMethod, y => y.MapFrom(z => z.RequestMethod))
			.ForMember(x => x.Id, y => y.Ignore())
			.ForMember(x => x.PostMethodOptionsId, y => y.Ignore());
		CreateMap<Request.PostMethodOptions, Share.Tables.PostMethodOptions>();
		CreateMap<Request.Query, Share.Tables.Query>();
		CreateMap<Request.Path, Share.Tables.Path>();
		CreateMap<Request.Header, Share.Tables.Header>();
		CreateMap<Request.ValueOptions, Share.Tables.ValueOptions>();
		CreateMap<Request.ValueItem, Share.Tables.ValueItem>();
		CreateMap<Request.Range, Share.Tables.Range>();
		CreateMap<Request.CreateParserTaskCommand, Share.Tables.ParserTask>()
			.ForMember(x => x.Id, y => y.Ignore())
			.ForMember(x => x.StatusId, y => y.MapFrom(z => 0))
			.ForMember(x => x.ParserTaskUrlOptionsId, y => y.Ignore())
			.ForMember(x => x.ParserTaskWebsiteTagsOptionsId, y => y.Ignore());
	}
}