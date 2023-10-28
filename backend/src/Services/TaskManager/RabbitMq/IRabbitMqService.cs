using Share.RabbitMessages;
using Share.Tables;
using TaskManager.ParserTasks.Commands.CreateParserTask.Response;

namespace TaskManager.RabbitMq;

public interface IRabbitMqService
{
    void SendNewTaskMessage(NewParserTaskMessage message);
}