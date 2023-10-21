namespace TaskManager.ParserTasks.Contracts.Core;

public class ParseWebsiteTagsOptions
{
    public Guid Id { get; set; }
    public IList<Tag> Tags { get; set; }
}

public class Tag
{
    public Guid Id { get; set; }
    public FindOptions FindOptions { get; set; }
}

public class FindOptions
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IList<TagAttribute> Attributes { get; set; }
}

public class TagAttribute
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}