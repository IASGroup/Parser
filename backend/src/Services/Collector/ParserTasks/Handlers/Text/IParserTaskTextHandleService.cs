using Share.RabbitMessages.ParserTaskAction;

namespace Collector.ParserTasks.Handlers.Text;

public interface IParserTaskTextHandleService
{
	Task Handle(ParserTask parserTaskInAction, CancellationToken cancellationToken);
}