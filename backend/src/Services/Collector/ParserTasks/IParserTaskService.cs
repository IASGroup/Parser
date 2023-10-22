using Core.Entities;

namespace Collector.ParserTasks;

public interface IParserTaskService
{
    Task NewTaskCreatedHandler(ParserTask newTask);
}