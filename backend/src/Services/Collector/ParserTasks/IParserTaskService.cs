using Share.RabbitMessages;

namespace Collector.ParserTasks;

public interface IParserTaskService
{
    Task NewTaskCreatedHandler(NewParserTaskMessage newTask);
}