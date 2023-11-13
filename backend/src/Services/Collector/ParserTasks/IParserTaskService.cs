using Share.RabbitMessages.ParserTaskAction;

namespace Collector.ParserTasks;

public interface IParserTaskService
{
	Task RunParserTaskHandler(ParserTask parserTaskInAction);
}