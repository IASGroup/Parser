using Share.RabbitMessages.ParserTaskAction;

namespace TaskManager.RabbitMq;

public interface IRabbitMqService
{
	void SendTaskActionMessage(ParserTaskActionMessage actionMessage);
}