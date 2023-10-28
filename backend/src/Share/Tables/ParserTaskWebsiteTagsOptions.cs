namespace Share.Tables;

public sealed class ParserTaskWebsiteTagsOptions
{
    public Guid Id { get; set; }
    
    public IEnumerable<ParserTaskWebsiteTag>? ParserTaskWebsiteTags { get; set; }
}

public sealed class ParserTaskWebsiteTag
{
    public Guid Id { get; set; }
    public Guid ParserTaskWebsiteTagsOptionsId { get; set; }
    public Guid FindOptionsId { get; set; }
    
    public FindOptions? FindOptions { get; set; }
    public ParserTaskWebsiteTagsOptions? ParserTaskWebsiteTagsOptions { get; set; }
}

public sealed class FindOptions
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    
    public IEnumerable<TagAttribute>? Attributes { get; set; }
}

public sealed class TagAttribute
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public Guid FindOptionsId { get; set; }

    public FindOptions? FindOptions { get; set; }
}