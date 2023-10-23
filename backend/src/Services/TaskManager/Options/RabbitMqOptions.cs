namespace TaskManager.Options;

public class RabbitMqOptions
{
    public static string Name = nameof(RabbitMqOptions);
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string UserPassword { get; set; }
    public string NewParserTasksQueueName { get; set; }
}