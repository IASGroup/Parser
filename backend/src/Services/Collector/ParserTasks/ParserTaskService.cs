using Collector.Contexts;
using Collector.RabbitMq;
using Core.Entities;

namespace Collector.ParserTasks;

public class ParserTaskService : IParserTaskService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, HttpMethod> _httpMethods = new()
    {
        {"GET", HttpMethod.Get},
        {"POST", HttpMethod.Post},
        {"PUT", HttpMethod.Put},
        {"DELETE", HttpMethod.Delete},
        {"PATCH", HttpMethod.Patch}
    };

    public ParserTaskService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task NewTaskCreatedHandler(ParserTask newTask)
    {
        await (newTask.Type.Id switch
        {
            1 => ParseApiHandler(newTask),
            _ => NotFoundTaskType(newTask)
        });
    }

    private async Task ParseApiHandler(ParserTask newTask)
    {
        using var taskScope = _serviceProvider.CreateScope();
        var dbContext = taskScope.ServiceProvider.GetService<AppDbContext>();
        using var httpClinet = new HttpClient();
        var request = new HttpRequestMessage()
        {
            Method = _httpMethods[newTask.ParserTaskUrlOptions.RequestMethod],
            RequestUri = new Uri(newTask.Url)
        };
        var response = await httpClinet.SendAsync(request);
        var rabbitMqService = taskScope.ServiceProvider.GetService<IRabbitMqService>();
        if (!response.IsSuccessStatusCode)
        {
            rabbitMqService.SendParserTaskCollectMessage(new()
            {
                ParserTaskId = newTask.Id,
                ParserTaskErrorMessage = new()
                {
                    ErrorMessage = $"Запрос вернул неуспешный код ответа: {response.StatusCode.ToString()}"
                }
            });
            newTask.StatusId = 4;
            dbContext.ParserTasks.Update(newTask);
            await dbContext.SaveChangesAsync();
            rabbitMqService.SendParserTaskCollectMessage(new()
            {
                ParserTaskId = newTask.Id,
                ParserTaskStatusChangedMessage = new()
                {
                    NewTaskStatus = newTask.StatusId
                }
            });
            return;
        }

        var contentString = await response.Content.ReadAsStringAsync();
        var newResult = new ParserTaskResult
        {
            ParserTaskId = newTask.Id,
            Content = contentString
        };
        dbContext.ParserTaskResults.Add(newResult);
        newTask.StatusId = 5;
        await dbContext.SaveChangesAsync();
        rabbitMqService.SendParserTaskCollectMessage(new()
        {
            ParserTaskId = newTask.Id,
            ParserTaskStatusChangedMessage = new()
            {
                NewTaskStatus = newTask.StatusId
            }
        });
    }

    private async Task NotFoundTaskType(ParserTask newTask)
    {

    }
}