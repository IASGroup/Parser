namespace Share.Tables;

public class ParserTaskPartialResult
{
    public Guid Id { get; set; }
    public Guid ParserTaskId { get; set; }
    public string? Url { get; set; }
    public int StatusId { get; set; }
    public string Content { get; set; } = null!;

    public ParserTask? ParserTask { get; set; }
    public ParserTaskPartialResultStatus? Status { get; set; }
}