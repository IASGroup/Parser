using MediatR;
using TaskManager.ParserTasks.Commands.CreateParserTask.Response;
using TaskManager.ParserTasks.Contracts;

namespace TaskManager.ParserTasks.Commands.CreateParserTask.Request;

public class CreateParserTaskCommand : IRequest<Result<CreateParserTaskResponseDto>>
{
	public string Url { get; set; } = null!;
	public int TypeId { get; set; }
	public string Name { get; set; } = null!;
	public ParserTaskWebsiteTagsOptions? ParserTaskWebsiteTagsOptions { get; set; }
	public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }

	public ParserTaskTorOptions? ParserTaskTorOptions { get; set; }
}

public class ParserTaskTorOptions
{
	public int ChangeIpAddressAfterRequestsNumber { get; set; }
}

public class ParserTaskUrlOptions
{
	public string RequestMethod { get; set; } = null!;
	public PostMethodOptions? PostMethodOptions { get; set; }
	public IEnumerable<Query>? Queries { get; set; }
	public IEnumerable<Path>? Paths { get; set; }
	public IEnumerable<Header>? Headers { get; set; }
}

public class PostMethodOptions
{
	public string RequestBody { get; set; } = null!;
}

public class Query
{
	public string Name { get; set; } = null!;
	public ValueOptions ValueOptions { get; set; } = null!;
}

public class Path
{
	public string Name { get; set; } = null!;
	public ValueOptions ValueOptions { get; set; } = null!;
}

public class Header
{
	public string Name { get; set; } = null!;
	public string Value { get; set; } = null!;
}

public class ParserTaskWebsiteTagsOptions
{
	public IEnumerable<ParserTaskWebsiteTag> ParserTaskWebsiteTags { get; set; } = null!;
}

public class ParserTaskWebsiteTag
{
	public FindOptions FindOptions { get; set; } = null!;
}

public class FindOptions
{
	public string Name { get; set; } = null!;
	public IEnumerable<TagAttribute>? Attributes { get; set; }
}

public class TagAttribute
{
	public string Name { get; set; } = null!;
	public string Value { get; set; } = null!;
}

public class ValueOptions
{
	public Range? Range { get; set; }
	public IEnumerable<ValueItem>? Values { get; set; }
	public string? Value { get; set; }
}

public class ValueItem
{
	public string Value { get; set; } = null!;
}

public class Range
{
	public int Start { get; set; }
	public int End { get; set; }
}