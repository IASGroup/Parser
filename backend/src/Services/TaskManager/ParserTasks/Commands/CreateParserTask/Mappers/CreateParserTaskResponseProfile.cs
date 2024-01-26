using AutoMapper;
using Tables = Share.Tables;

namespace TaskManager.ParserTasks.Commands.CreateParserTask.Mappers;

public class CreateParserTaskResponseProfile : Profile
{
	public CreateParserTaskResponseProfile()
	{
		CreateMap<Tables.ParserTaskWebsiteTagsOptions, Response.ParserTaskWebsiteTagsOptions>();
		CreateMap<Tables.ParserTaskWebsiteTag, Response.ParserTaskWebsiteTag>();
		CreateMap<Tables.FindOptions, Response.FindOptions>();
		CreateMap<Tables.TagAttribute, Response.TagAttribute>();
		CreateMap<Tables.ParserTaskUrlOptions, Response.ParserTaskUrlOptions>();
		CreateMap<Tables.PostMethodOptions, Response.PostMethodOptions>();
		CreateMap<Tables.Query, Response.Query>();
		CreateMap<Tables.Path, Response.Path>();
		CreateMap<Tables.Header, Response.Header>();
		CreateMap<Tables.ValueOptions, Response.ValueOptions>();
		CreateMap<Tables.ValueItem, Response.ValueItem>();
		CreateMap<Tables.Range, Response.Range>();
		CreateMap<Tables.ParserTask, Response.CreateParserTaskResponseDto>();
		CreateMap<Tables.ParserTaskTorOptions, Response.ParserTaskTorOptions>();
	}
}