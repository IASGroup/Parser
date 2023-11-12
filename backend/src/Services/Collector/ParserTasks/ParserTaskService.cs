using System.Text.Json;
using Collector.Contexts;
using Collector.Contracts;
using Collector.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Share.Contracts;
using Share.RabbitMessages;
using Share.Tables;
using ParserTaskStatuses = Share.Contracts.ParserTaskStatuses;

namespace Collector.ParserTasks;

public class ParserTaskService : IParserTaskService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ParserTaskService> _logger;
    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, HttpMethod> _httpMethods = new()
    {
        {"GET", HttpMethod.Get},
        {"POST", HttpMethod.Post},
        {"PUT", HttpMethod.Put},
        {"DELETE", HttpMethod.Delete},
        {"PATCH", HttpMethod.Patch}
    };

    public ParserTaskService(
        IServiceProvider serviceProvider,
        ILogger<ParserTaskService> logger,
        IHttpClientFactory httpClientFactory
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(HttpClientNames.Collector);
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

            foreach (var url in GetParserTaskUrls(newTask))
            {
                var request = new HttpRequestMessage
                {
                    Method = _httpMethods[newTask.ParserTaskUrlOptions!.RequestMethod],
                    RequestUri = new Uri(url)
                };
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    rabbitMqService.SendParserTaskCollectMessage(new()
                    {
                        ParserTaskId = newTask.Id,
                        ParserTaskErrorMessage = new ParserTaskErrorMessage
                        {
                            ErrorMessage = JsonSerializer.Serialize(new
                            {
                                StatusCode = response.StatusCode,
                                Content = responseContent
                            }),
                            Url = url
                        }
                    });
                    dbContext.ParserTaskPartialResults.Add(new ParserTaskPartialResult()
                    {
                        StatusId = (int)ParserTaskPartialResultStatuses.Error,
                        Url = url,
                        Content = responseContent,
                        ParserTaskId = newTask.Id
                    });
                    await dbContext.SaveChangesAsync();
                    return;
                }
                var newResult = new ParserTaskPartialResult
                {
                    ParserTaskId = newTask.Id,
                    Url = url,
                    StatusId = (int)ParserTaskPartialResultStatuses.Success,
                    Content = responseContent
                };
                dbContext.ParserTaskPartialResults.Add(newResult);
                await dbContext.SaveChangesAsync();
            }
            await dbContext.ParserTasks
                .Where(x => x.Id == newTask.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(
                    y => y.StatusId, (int)ParserTaskStatuses.Finished)
                );
            rabbitMqService.SendParserTaskCollectMessage(new()
            {
                ParserTaskId = newTask.Id,
                ParserTaskStatusChangedMessage = new()
                {
                    NewTaskStatus = (int)ParserTaskStatuses.Finished
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

    private enum UrlPartTypes
    {
        Path = 0,
        Query = 1
    }

    private class UrlPart
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public UrlPartTypes PartType { get; set; }
    }

    private IEnumerable<string> GetParserTaskUrls(NewParserTaskMessage message)
    {
        var paths = message.ParserTaskUrlOptions!.Paths!.Select(x =>
        {
            if (x.ValueOptions.Value is not null) return new[]
            {
                new UrlPart()
                {
                    Name = x.Name,
                    Value = x.ValueOptions.Value,
                    PartType = UrlPartTypes.Path
                }
            };
            if (x.ValueOptions.Values is not null && x.ValueOptions.Values.Any())
            {
                return x.ValueOptions.Values.Select(y => new UrlPart()
                {
                    Name = x.Name,
                    Value = y.Value,
                    PartType = UrlPartTypes.Path
                }).ToArray();
            }

            return Enumerable.Range(
                    start: x.ValueOptions.Range!.Start,
                    count: x.ValueOptions.Range.End - x.ValueOptions.Range.Start + 1
                )
                .Select(y => new UrlPart()
                {
                    Name = x.Name,
                    Value = y.ToString(),
                    PartType = UrlPartTypes.Path
                }).ToArray();
        });

        var queries = message.ParserTaskUrlOptions!.Queries!.Select(x =>
        {
            if (x.ValueOptions.Value is not null) return new[]
            {
                new UrlPart()
                {
                    Name = x.Name,
                    Value = x.ValueOptions.Value,
                    PartType = UrlPartTypes.Query
                }
            };
            if (x.ValueOptions.Values is not null && x.ValueOptions.Values.Any())
            {
                return x.ValueOptions.Values.Select(y => new UrlPart()
                {
                    Name = x.Name,
                    Value = y.Value,
                    PartType = UrlPartTypes.Query
                }).ToArray();
            }

            return Enumerable.Range(
                    start: x.ValueOptions.Range!.Start,
                    count: x.ValueOptions.Range.End - x.ValueOptions.Range.Start + 1
                )
                .Select(y => new UrlPart()
                {
                    Name = x.Name,
                    Value = y.ToString(),
                    PartType = UrlPartTypes.Query
                }).ToArray();
        });

        var parts = paths.Concat(queries).ToArray();

        var indexes = new int[parts.Length];
        var currentArrayIndex = indexes.Length - 1;
        for (var i = 0; i < indexes.Length; i++)
        {
            indexes[i] = 0;
        }

        bool IsIndexesEnds()
        {
            for (var i = 0; i < indexes.Length; i++)
            {
                if (indexes[i] != parts[i].Length - 1) return false;
            }

            return true;
        }

        string GetCurrentUrl()
        {
            var defaultPath = message.Url;
            var indexOfQuery = defaultPath.IndexOf('?');
            defaultPath = indexOfQuery == -1
                ? defaultPath
                : defaultPath.Remove(indexOfQuery, defaultPath.Length - indexOfQuery);
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i][indexes[i]];
                if (part.PartType is UrlPartTypes.Path)
                {
                    defaultPath = defaultPath.Replace($"{{{part.Name}}}", part.Value);
                }
                else
                {
                    defaultPath += defaultPath.Contains('?') ? "&" : "?" + $"{part.Name}={part.Value}";
                }
            }

            return defaultPath;
        }

        while (!IsIndexesEnds())
        {
            var subArrayIndex = indexes[currentArrayIndex];
            if (subArrayIndex < parts[currentArrayIndex].Length - 1)
            {
                yield return GetCurrentUrl();
                indexes[currentArrayIndex] = subArrayIndex + 1;
                if (currentArrayIndex < parts.Length - 1)
                {
                    for (int i = currentArrayIndex + 1; i < parts.Length; i++)
                    {
                        indexes[i] = 0;
                    }

                    currentArrayIndex = indexes.Length - 1;
                }
            }
            else if (currentArrayIndex != 0)
            {
                currentArrayIndex = currentArrayIndex - 1;
            }
        }

        yield return GetCurrentUrl();
    }
}