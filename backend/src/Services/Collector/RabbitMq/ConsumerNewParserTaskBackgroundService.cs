using System.Text;
using System.Text.Json;
using Collector.Options;
using Collector.ParserTasks;
using Core.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
        _channel.QueueDeclare(_rabbitMqOptions.NewParserTasksQueueName, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (sender, args) =>
        {
            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            var parserTask = JsonSerializer.Deserialize<ParserTask>(message);
            await _parserTaskService.NewTaskCreatedHandler(parserTask);
            _channel.BasicAck(args.DeliveryTag, false);
            await Task.Yield();
        };
        _channel.BasicConsume(_rabbitMqOptions.NewParserTasksQueueName, false, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_channel.IsOpen) _channel.Close();
        if (_connection.IsOpen) _connection.Close();
        base.Dispose();
    }
}