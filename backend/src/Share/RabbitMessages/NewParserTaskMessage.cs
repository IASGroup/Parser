namespace Share.RabbitMessages;

public class NewParserTaskMessage
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public int TypeId { get; set; }
    public string Name { get; set; } = null!;
    public int StatusId { get; set; }
    public ParserTaskWebsiteTagsOptions? ParserTaskWebsiteTagsOptions { get; set; }
    public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }
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
    public IEnumerable<TagAttribute> Attributes { get; set; } = null!;
}

public class TagAttribute
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public class ParserTaskUrlOptions
{
    public string RequestMethod { get; set; } = null!;
    public PostMethodOptions? PostMethodOptions { get; set; }
    public IEnumerable<Query>? Queries { get; set; } = null!;
    public IEnumerable<Path>? Paths { get; set; } = null!;
    public IEnumerable<Header>? Headers { get; set; } = null!;
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
public class ValueOptions
{
    public Range Range { get; set; } = null!;
    public IEnumerable<ValueItem> Values { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public class ValueItem
{
    public string Value { get; set; }
}

public class Range
{
    public int Start { get; set; }
    public int End { get; set; }
}