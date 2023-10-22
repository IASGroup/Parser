namespace Core.Entities;

public class ParserTaskResult
{
    public Guid Id { get; set; }
    public Guid ParserTaskId { get; set; }
    public ParserTask ParserTask { get; set; }
    public string Content { get; set; }
}