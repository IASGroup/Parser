namespace Reporter.ParserTask.Queries.GetTasks;

public class ParserTaskListItemDto
{
	public Guid Id { get; set; }
	public string Url { get; set; } = null!;
	public string Name { get; set; } = null!;
	public int TypeId { get; set; }
	public int StatusId { get; set; }
	public bool HasErrors { get; set; }
	public int CompletedPartsNumber { get; set; }
	public int AllPartsNumber { get; set; }
}