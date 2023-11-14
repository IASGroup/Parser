using System.Text;
using System.Text.Json;
using Hubs.Contracts;
using Hubs.Hubs;
using Hubs.Options;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hubs.RabbitMq;

public class ConsumerParserTaskCollectMessagesBackgroundService : BackgroundService
{
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly RabbitMqOptions _rabbitMqOptions;
	private readonly IHubContext<ParseTasksHub> _appHub;

	public ConsumerParserTaskCollectMessagesBackgroundService(
		IConfiguration configuration,
		ILogger<ConsumerParserTaskCollectMessagesBackgroundService> logger,
		IServiceProvider serviceProvider,
		IHubContext<ParseTasksHub> appHub
	)
	{
		_rabbitMqOptions = configuration.GetSection(RabbitMqOptions.Name).Get<RabbitMqOptions>()!;
		_appHub = appHub;
		var factory = new ConnectionFactory
		{
			HostName = _rabbitMqOptions.HostName,
			UserName = _rabbitMqOptions.UserName,
			Password = _rabbitMqOptions.UserPassword,
			DispatchConsumersAsync = true
		};
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();
		_channel.QueueDeclare(_rabbitMqOptions.ParserTaskCollectMessagesQueueName, exclusive: false, autoDelete: false);
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.Received += ConsumerOnReceived;
		_channel.BasicConsume(_rabbitMqOptions.ParserTaskCollectMessagesQueueName, false, consumer);
		return Task.CompletedTask;
	}

	private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs args)
	{
		var message = Encoding.UTF8.GetString(args.Body.ToArray());
		var collectMessage = JsonSerializer.Deserialize<ParserTaskCollectMessage>(message);
		await _appHub.Clients.All.SendAsync("NewParserTaskCollectMessage", collectMessage);
		_channel.BasicAck(args.DeliveryTag, false);
		await Task.Yield();
	}
}