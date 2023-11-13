using Share.RabbitMessages.ParserTaskAction;

namespace Collector.ParserTasks.Handlers.Api;

public interface IParserTaskApiHandleService
{
	Task Handle(ParserTask parserTaskInAction);
}