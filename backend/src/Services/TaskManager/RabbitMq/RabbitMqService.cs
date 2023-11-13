using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Share.RabbitMessages;
using Share.RabbitMessages.ParserTaskAction;
using Share.Tables;
using TaskManager.Options;
using TaskManager.ParserTasks.Commands.CreateParserTask.Response;

namespace TaskManager.RabbitMq;

public class RabbitMqService : IRabbitMqService
{
	private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;

	public RabbitMqService(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions)
	{
		_rabbitMqOptions = rabbitMqOptions;
	}

	public void SendTaskActionMessage(ParserTaskActionMessage actionMessage)
	{
		var json = JsonSerializer.Serialize(actionMessage);
		var factory = new ConnectionFactory
		{
			HostName = _rabbitMqOptions.Value.HostName,
			UserName = _rabbitMqOptions.Value.UserName,
			Password = _rabbitMqOptions.Value.UserPassword
		};
		using var connection = factory.CreateConnection();
		using var channel = connection.CreateModel();
		channel.QueueDeclare(_rabbitMqOptions.Value.ParserTaskActionsQueueName, exclusive: false, autoDelete: false);
		var body = Encoding.UTF8.GetBytes(json);
		channel.BasicPublish(
			exchange: "",
			routingKey: _rabbitMqOptions.Value.ParserTaskActionsQueueName,
			body: body
		);
	}
}