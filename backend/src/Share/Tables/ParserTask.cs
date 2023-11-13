namespace Share.Tables;

public sealed class ParserTask
{
	public Guid Id { get; set; }
	public string Url { get; set; } = null!;
	public int TypeId { get; set; }
	public string Name { get; set; } = null!;
	public int StatusId { get; set; }
	public Guid? ParserTaskWebsiteTagsOptionsId { get; set; }
	public Guid? ParserTaskUrlOptionsId { get; set; }

	public ParserTaskType? Type { get; set; }
	public ParserTaskStatuses? Status { get; set; }
	public ParserTaskWebsiteTagsOptions? ParserTaskWebsiteTagsOptions { get; set; }
	public ParserTaskUrlOptions? ParserTaskUrlOptions { get; set; }
}

public sealed class ParserTaskType
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string Description { get; set; } = null!;

	public IEnumerable<ParserTask>? ParserTasks { get; set; }
}