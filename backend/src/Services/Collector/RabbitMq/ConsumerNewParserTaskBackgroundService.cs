using System.Text;
using System.Text.Json;
using Collector.Contracts.Core;
using Collector.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Collector.RabbitMq;

public class ConsumerNewParserTaskBackgroundService : BackgroundService
{
    private readonly RabbitMqOptions _rabbitMqOptions;
    private readonly ILogger<ConsumerNewParserTaskBackgroundService> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public ConsumerNewParserTaskBackgroundService(
        IConfiguration configuration,
        ILogger<ConsumerNewParserTaskBackgroundService> logger
    )
    {
        _rabbitMqOptions = configuration.GetSection(RabbitMqOptions.Name).Get<RabbitMqOptions>()!;
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.UserPassword
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(_rabbitMqOptions.ParserTasksQueryName, exclusive: false, autoDelete: false);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (sender, args) =>
        {
            var message = Encoding.UTF8.GetString(args.Body.ToArray());
            var parserTask = JsonSerializer.Deserialize<ParserTask>(message);
            _logger.LogInformation(message);
            Console.WriteLine("Test");
            _channel.BasicAck(args.DeliveryTag, false);
        };
        _channel.BasicConsume(_rabbitMqOptions.ParserTasksQueryName, false, consumer);
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_channel.IsOpen) _channel.Close();
        if (_connection.IsOpen) _connection.Close();
        base.Dispose();
    }
}