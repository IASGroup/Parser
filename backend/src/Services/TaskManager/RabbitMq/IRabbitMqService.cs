namespace TaskManager.RabbitMq;

public interface IRabbitMqService
{
    void SendMessage<T>(T message);
}