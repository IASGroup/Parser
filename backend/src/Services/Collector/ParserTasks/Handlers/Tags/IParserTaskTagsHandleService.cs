using Share.RabbitMessages.ParserTaskAction;

namespace Collector.ParserTasks.Handlers.Tags;

public interface IParserTaskTagsHandleService
{
	Task Handle(ParserTask parserTaskInAction, CancellationToken cancellationToken);
}