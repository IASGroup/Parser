namespace Share.RabbitMessages.ParserTaskAction;

public class ParserTaskActionMessage
{
	public ParserTask ParserTask { get; set; } = null!;
	public ParserTaskActions ParserTaskAction { get; set; }
}