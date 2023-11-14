using Share.RabbitMessages.ParserTaskAction;

namespace Collector.ParserTasks;

public interface IParserTaskService
{
	Task HandleRunParserTaskMessageAsync(ParserTask parserTaskInAction);

	Task HandleStopParserTaskMessageAsync(ParserTask parserTaskInAction);
}