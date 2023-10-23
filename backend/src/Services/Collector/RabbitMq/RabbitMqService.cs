using System.Text;
using System.Text.Json;
using Collector.Contracts;
using Collector.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Collector.RabbitMq;

public class RabbitMqService : IRabbitMqService
{
    private readonly IOptionsSnapshot<RabbitMqOptions> _rabbitMqOptions;

    public RabbitMqService(IOptionsSnapshot<RabbitMqOptions> rabbitMqOptions)
    {
        _rabbitMqOptions = rabbitMqOptions;
    }
    
    public void SendParserTaskCollectMessage(ParserTaskCollectMessage parserTaskCollectMessage)
    {
        var json = JsonSerializer.Serialize(parserTaskCollectMessage);
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.Value.HostName,
            UserName = _rabbitMqOptions.Value.UserName,
            Password = _rabbitMqOptions.Value.UserPassword
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(_rabbitMqOptions.Value.ParserTaskCollectMessagesQueueName, exclusive: false, autoDelete: false);
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(
            exchange: "",
            routingKey: _rabbitMqOptions.Value.ParserTaskCollectMessagesQueueName,
            body: body
        );
    }
}