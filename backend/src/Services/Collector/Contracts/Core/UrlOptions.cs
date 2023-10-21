namespace Collector.Contracts.Core;

public class UrlOptions
{
    public Guid Id { get; set; }
    public string RequestMethod { get; set; }
    public PostMethodOptions? PostMethodOptions { get; set; }
    public IList<Query> Queries { get; set; }
    public IList<Path> Paths { get; set; }
    public IList<Header> Headers { get; set; }
}

public class PostMethodOptions
{
    public Guid Id { get; set; }
    public string RequestBody { get; set; }
}

public class Query
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ValueOptions ValueOptions { get; set; }
}

public class Path
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ValueOptions ValueOptions { get; set; }
}

public class Header
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}