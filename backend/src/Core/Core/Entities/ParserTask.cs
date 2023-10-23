namespace Core.Entities;

public class ParserTask
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public ParserTaskType Type { get; set; }
    public string Name { get; set; }
    public ParserTaskWebsiteTagsOptions? ParserTaskWebsiteTagsOptions { get; set; }
    public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }
}

public class ParserTaskType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}