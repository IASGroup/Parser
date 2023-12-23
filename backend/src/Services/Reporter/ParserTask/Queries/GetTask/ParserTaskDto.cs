namespace Reporter.ParserTask.Queries.GetTask;

public class ParserTaskDto
{
	public Guid Id { get; set; }
	public string Url { get; set; } = null!;
	public string Name { get; set; } = null!;
	public int TypeId { get; set; }
	public int StatusId { get; set; }
	public string? InProgressUrl { get; set; }
	public int AllPartsNumber { get; set; }
	public int CompletedPartsNumber { get; set; }
}