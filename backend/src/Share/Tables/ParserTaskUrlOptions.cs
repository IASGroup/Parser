namespace Share.Tables;

public class ParserTaskUrlOptions
{
    public Guid Id { get; set; }
    public string RequestMethod { get; set; } = null!;
    public Guid? PostMethodOptionsId { get; set; }

    public PostMethodOptions? PostMethodOptions { get; set; }
    public IEnumerable<Query>? Queries { get; set; }
    public IEnumerable<Path>? Paths { get; set; }
    public IEnumerable<Header>? Headers { get; set; }
}

public sealed class PostMethodOptions
{
    public Guid Id { get; set; }
    public string RequestBody { get; set; } = null!;

    public IEnumerable<ParserTaskUrlOptions>? ParserTaskUrlOptions { get; set; }
}

public sealed class Query
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ValueOptionsId { get; set; }
    public Guid? ParserTaskUrlOptionsId { get; set; }

    public ValueOptions? ValueOptions { get; set; }
    public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }
}

public sealed class Path
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid ValueOptionsId { get; set; }
    public Guid ParserTaskUrlOptionsId { get; set; }

    public ValueOptions? ValueOptions { get; set; }
    public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }
}

public sealed class Header
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public Guid ParserTaskUrlOptionsId { get; set; }

    public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }
}