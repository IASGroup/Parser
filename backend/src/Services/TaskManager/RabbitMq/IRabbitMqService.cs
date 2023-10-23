using Core.Entities;

namespace TaskManager.RabbitMq;

public interface IRabbitMqService
{
    void SendNewTaskMessage(ParserTask newTask);
}