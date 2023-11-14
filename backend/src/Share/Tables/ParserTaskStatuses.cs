namespace Share.Tables;

public sealed class ParserTaskStatuses
{
	public int Id { get; set; }
	public string Key { get; set; } = null!;
	public string Description { get; set; } = null!;

	public IEnumerable<ParserTask>? ParserTasks { get; set; }
}