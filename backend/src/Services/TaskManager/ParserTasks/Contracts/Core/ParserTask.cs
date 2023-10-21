namespace TaskManager.ParserTasks.Contracts.Core;

public class ParserTask
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public ParserTaskType Type { get; set; }
    public string Name { get; set; }
    public ParseWebsiteTagsOptions? WebsiteTagOptions { get; set; }
    public UrlOptions? UrlOptions { get; set; }
}

public class ParserTaskType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}