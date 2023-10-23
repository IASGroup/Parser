namespace TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation;

public class ParserTaskUrlOptions
{
    public string RequestMethod { get; set; }
    public PostMethodOptions? PostMethodOptions { get; set; }
    public IList<Query> Queries { get; set; } = new List<Query>();
    public IList<Path> Paths { get; set; } = new List<Path>();
    public IList<Header> Headers { get; set; } = new List<Header>();
}

public class PostMethodOptions
{
    public string RequestBody { get; set; }
}

public class Query
{
    public string Name { get; set; }
    public ValueOptions ValueOptions { get; set; }
}

public class Path
{
    public string Name { get; set; }
    public ValueOptions ValueOptions { get; set; }
}

public class Header
{
    public string Name { get; set; }
    public string Value { get; set; }
}