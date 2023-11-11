namespace Hubs.Options;

public class RabbitMqOptions
{
    public static string Name = nameof(RabbitMqOptions);
    public string HostName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string UserPassword { get; set; } = null!;
    public string ParserTaskCollectMessagesQueueName { get; set; } = null!;
}