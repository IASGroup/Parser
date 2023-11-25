namespace Reporter.ParserTask.Queries.GetTaskResults;

public class ParserTaskResult
{
	public Guid Id { get; set; }
	public Guid ParserTaskId { get; set; }
	public string Url { get; set; } = null!;
	public int StatusId { get; set; }
}