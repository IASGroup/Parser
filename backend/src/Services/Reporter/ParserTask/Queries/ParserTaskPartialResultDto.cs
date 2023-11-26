namespace Reporter.ParserTask.Queries;

public class ParserTaskPartialResultDto
{
	public Guid Id { get; set; }
	public Guid ParserTaskId { get; set; }
	public string? Url { get; set; }
	public int? StatusId { get; set; }
	public string? Content { get; set; }
}