using Collector.Contexts;
using Collector.Contracts;
using Collector.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Share.RabbitMessages;
using Share.Tables;
using ParserTaskStatuses = Share.Contracts.ParserTaskStatuses;

namespace Collector.ParserTasks;

public class ParserTaskService : IParserTaskService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ParserTaskService> _logger;

    private readonly Dictionary<string, HttpMethod> _httpMethods = new()
    {
        {"GET", HttpMethod.Get},
        {"POST", HttpMethod.Post},
        {"PUT", HttpMethod.Put},
        {"DELETE", HttpMethod.Delete},
        {"PATCH", HttpMethod.Patch}
    };

    public ParserTaskService(IServiceProvider serviceProvider, ILogger<ParserTaskService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task NewTaskCreatedHandler(NewParserTaskMessage newTask)
    {
        await (newTask.TypeId switch
        {
            1 => ParseApiHandler(newTask),
            _ => NotFoundTaskType(newTask)
        });
    }

    private async Task ParseApiHandler(NewParserTaskMessage newTask)
    {
        try
        {
            _logger.LogInformation($"Начало задачи парсинга: {newTask.Id}");
            using var taskScope = _serviceProvider.CreateScope();
            var dbContext = taskScope.ServiceProvider.GetService<AppDbContext>();
            var rabbitMqService = taskScope.ServiceProvider.GetService<IRabbitMqService>();

            if (dbContext is null || rabbitMqService is null)
            {
                _logger.LogError(
                    message: "Необходимый сервис не найден",
                    args: new { dbContext, rabbitMqService }
                );
                return;
            }
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage()
            {
                Method = _httpMethods[newTask.ParserTaskUrlOptions!.RequestMethod],
                RequestUri = new Uri(newTask.Url)
            };
            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                rabbitMqService.SendParserTaskCollectMessage(new()
                {
                    ParserTaskId = newTask.Id,
                    ParserTaskErrorMessage = new ParserTaskErrorMessage
                    {
                        ErrorMessage = $"Запрос вернул неуспешный код ответа: {response.StatusCode.ToString()}"
                    }
                });
                await dbContext.ParserTasks
                    .Where(x => x.Id == newTask.Id)
                    .ExecuteUpdateAsync(x => x.SetProperty(
                        y => y.StatusId, (int)ParserTaskStatuses.Error)
                    );
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
            await dbContext.ParserTasks
                .Where(x => x.Id == newTask.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(
                    y => y.StatusId, (int)ParserTaskStatuses.Finished)
                );
            await dbContext.SaveChangesAsync();
            rabbitMqService.SendParserTaskCollectMessage(new()
            {
                ParserTaskId = newTask.Id,
                ParserTaskStatusChangedMessage = new()
                {
                    NewTaskStatus = newTask.StatusId
                }
            });
            _logger.LogInformation($"Конец задачи парсинга: {newTask.Id}");
        }
        catch (Exception e)
        {
            var errorMessage = $"Ошибка при выполнении задачи парсинга: {newTask.Name}";
            _logger.LogError(
                exception: e,
                message: errorMessage,
                args: new { newTask }
            );
        }
    }

    private async Task NotFoundTaskType(NewParserTaskMessage newTask)
    {
    }
}