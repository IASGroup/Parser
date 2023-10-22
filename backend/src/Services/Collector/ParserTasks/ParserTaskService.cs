using Collector.Contexts;
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
        using var taskScope = _serviceProvider.CreateScope();
        using var httpClinet = new HttpClient();
        var request = new HttpRequestMessage()
        {
            Method = _httpMethods[newTask.ParserTaskUrlOptions.RequestMethod],
            RequestUri = new Uri(newTask.Url)
        };
        var response = await httpClinet.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {

        }

        var contentString = await response.Content.ReadAsStringAsync();
        var dbContext = taskScope.ServiceProvider.GetService<AppDbContext>();
        var newResult = new ParserTaskResult()
        {
            ParserTaskId = newTask.Id,
            Content = contentString
        };
        dbContext.ParserTaskResults.Add(newResult);
        await dbContext.SaveChangesAsync();
        // TODO: Отправить уведомление о завершении задачи парсинга
    }
}