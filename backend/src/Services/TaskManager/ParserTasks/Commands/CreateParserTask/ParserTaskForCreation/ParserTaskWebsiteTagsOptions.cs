namespace TaskManager.ParserTasks.Commands.CreateParserTask.ParserTaskForCreation;

public class ParserTaskWebsiteTagsOptions
{
    public IList<ParserTaskWebsiteTag> ParserTaskWebsiteTags { get; set; }
}

public class ParserTaskWebsiteTag
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