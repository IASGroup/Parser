namespace Share.Tables;

public class ParserTaskResult
{
    public Guid Id { get; set; }
    public Guid ParserTaskId { get; set; }
    public string Content { get; set; } = null!;

    public ParserTask? ParserTask { get; set; }
}