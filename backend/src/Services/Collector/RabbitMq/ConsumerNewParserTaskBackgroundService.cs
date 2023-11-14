using System.Text;
using System.Text.Json;
using Collector.Options;
using Collector.ParserTasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Share.RabbitMessages.ParserTaskAction;

namespace Collector.RabbitMq;

public class ConsumerNewParserTaskBackgroundService : BackgroundService
{
	private readonly RabbitMqOptions _rabbitMqOptions;
	private readonly ILogger<ConsumerNewParserTaskBackgroundService> _logger;
	private readonly IParserTaskService _parserTaskService;
	private readonly IConnection _connection;
	private readonly IModel _channel;

	public ConsumerNewParserTaskBackgroundService(
		IConfiguration configuration,
		ILogger<ConsumerNewParserTaskBackgroundService> logger,
		IParserTaskService parserTaskService
	)
	{
		_rabbitMqOptions = configuration.GetSection(RabbitMqOptions.Name).Get<RabbitMqOptions>()!;
		_logger = logger;
		_parserTaskService = parserTaskService;
		var factory = new ConnectionFactory
		{
			HostName = _rabbitMqOptions.HostName,
			UserName = _rabbitMqOptions.UserName,
			Password = _rabbitMqOptions.UserPassword,
			DispatchConsumersAsync = true
		};
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();
		_channel.QueueDeclare(_rabbitMqOptions.ParserTaskActionsQueueName, exclusive: false, autoDelete: false);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.Received += async (sender, args) =>
		{
			try
			{
				var message = Encoding.UTF8.GetString(args.Body.ToArray());
				var parserTask = JsonSerializer.Deserialize<ParserTaskActionMessage>(message);
				_ = parserTask switch
				{
					{ ParserTaskAction: ParserTaskActions.Run }
						=> _parserTaskService.HandleRunParserTaskMessageAsync(parserTask.ParserTask),
					{ ParserTaskAction: ParserTaskActions.Pause }
						=> _parserTaskService.HandleStopParserTaskMessageAsync(parserTask.ParserTask),
					_ => Task.CompletedTask
				};
				_channel.BasicAck(args.DeliveryTag, false);
				await Task.Yield();
			}
			catch (Exception e)
			{
				var errorMessage = "Произошла ошибка при получении сообщения";
				_logger.LogError(
					exception: e,
					message: errorMessage,
					args: new { sender, args }
				);
			}
		};
		_channel.BasicConsume(_rabbitMqOptions.ParserTaskActionsQueueName, false, consumer);
		await Task.Delay(2000, stoppingToken);
	}

	public override void Dispose()
	{
		if (_channel.IsOpen) _channel.Close();
		if (_connection.IsOpen) _connection.Close();
		base.Dispose();
	}
}