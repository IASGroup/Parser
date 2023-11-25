namespace Hubs.Contracts;

public class ParserTaskCollectMessage
{
	public Guid ParserTaskId { get; set; }
	public required ParserTaskCollectMessageTypes Type { get; set; }
	public ParserTaskStatusChangedMessage? ParserTaskStatusChangedMessage { get; set; }
	public ParserTaskErrorMessage? ParserTaskErrorMessage { get; set; }
	public ParserTaskProgressMessage? ParserTaskProgressMessage { get; set; }
}

public enum ParserTaskCollectMessageTypes
{
	StatusChanged,
	Progress
}

public class ParserTaskProgressMessage
{
	public string CompletedPartUrl { get; set; } = null!;
	public string? NextPartUrl { get; set; }
	public int CompletedPartsNumber { get; set; }
}

public class ParserTaskErrorMessage
{
	public string? Url { get; set; }
	public string ErrorMessage { get; set; } = null!;
}

public class ParserTaskStatusChangedMessage
{
	public int NewTaskStatus { get; set; }
}