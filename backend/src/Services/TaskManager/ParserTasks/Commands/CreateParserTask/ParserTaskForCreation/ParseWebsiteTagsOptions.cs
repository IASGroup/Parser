namespace TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation;

public class ParseWebsiteTagsOptions
{
    public IList<Tag> Tags { get; set; }
}

public class Tag
{
    public FindOptions FindOptions { get; set; }
}

public class FindOptions
{
    public string Name { get; set; }
    public IList<TagAttribute> Attributes { get; set; }
}

public class TagAttribute
{
    public string Name { get; set; }
    public string Value { get; set; }
}