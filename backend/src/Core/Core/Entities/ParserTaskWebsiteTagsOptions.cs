namespace Core.Entities;

public class ParserTaskWebsiteTagsOptions
{
    public Guid Id { get; set; }
    public IList<ParserTaskWebsiteTag> ParserTaskWebsiteTags { get; set; }
}

public class ParserTaskWebsiteTag
{
    public Guid Id { get; set; }
    public Guid ParserWebsiteTags { get; set; }
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