using System.Text;
using Newtonsoft.Json;
using TaskManager.ParserTasks.Commands.CreateParserTask.Response;
using TaskManager.ParserTasks.Commands.RunParserTask.Response;
using TaskManager.ParserTasks.Commands.StopParserTask.Response;
using JsonSerializer = System.Text.Json.JsonSerializer;
using ParserTaskStatuses = Share.Contracts.ParserTaskStatuses;
using ParserTaskUrlOptions = Share.Tables.ParserTaskUrlOptions;

namespace Tests;

public class Tests
{
	private CreateParserTaskResponseDto createdTask = null!;

	/// <summary>
	/// Тест создания задачи парсинга и проверки что созданная задача парсинга имеет статус Created
	/// </summary>
	[Test]
	public void ParsingTaskMustBeCreatedWithStatusCreated()
	{
		// Формируем запрос на создание задачи парсинга 
		using var httpClient = new HttpClient();
		var request = new HttpRequestMessage()
		{
			Method = HttpMethod.Post,
			RequestUri = new Uri("http://localhost:5053/api/parser-tasks"),
			Content = new StringContent(JsonSerializer.Serialize(new
			{
				Name = "Спарсить фейк апи",
				Url = "https://jsonplaceholder.typicode.com/todos",
				TypeId = 1,
				ParserTaskUrlOptions = new ParserTaskUrlOptions
				{
					RequestMethod = "GET"
				}
			}), Encoding.UTF8, "application/json")
		};
		var response = httpClient.Send(request);
		var responseContent = response.Content.ReadAsStringAsync().Result;
		Assert.That(response.IsSuccessStatusCode, Is.EqualTo(true));
		createdTask = JsonConvert.DeserializeObject<CreateParserTaskResponseDto>(responseContent)!;
		Assert.That(createdTask, Is.Not.EqualTo(null));
		
		// Проверяем что задача парсинга имеет статус Created
		Assert.That(createdTask.StatusId, Is.EqualTo((int) ParserTaskStatuses.Created));
	}

	/// <summary>
	/// Тест запуска задачи парсинга
	/// </summary>
	[Test]
	public void ParsingTaskMustStartAndHaveStatusInProgress()
	{
		Assert.That(createdTask, Is.Not.EqualTo(null));
		
		// Формируем запрос на запуск задачи парсинга 
		using var httpClient = new HttpClient();
		var request = new HttpRequestMessage()
		{
			Method = HttpMethod.Post,
			RequestUri = new Uri($"http://localhost:5053/api/parser-tasks/{createdTask.Id}/run")
		};
		var runTaskResponse = httpClient.Send(request);
		var runTaskResponseContent = runTaskResponse.Content.ReadAsStringAsync().Result;
		Assert.That(runTaskResponse.IsSuccessStatusCode, Is.EqualTo(true));
		var runTaskResponseDto = JsonConvert.DeserializeObject<RunParserTaskResponseDto>(runTaskResponseContent)!;
		Assert.That(runTaskResponseDto, Is.Not.EqualTo(null));
		
		// Проверяем что id запущенной задачи парсинга совпадает с созданной
		Assert.That(runTaskResponseDto.ParserTaskId, Is.EqualTo(createdTask.Id));
	}
	
	/// <summary>
	/// Тест остановки задачи парсинга
	/// </summary>
	[Test]
	public void ParsingTaskShouldStopOnRequest()
	{
		Assert.That(createdTask, Is.Not.EqualTo(null));
		
		// Формируем запрос на остановку задачи парсинга 
		using var httpClient = new HttpClient();
		var request = new HttpRequestMessage()
		{
			Method = HttpMethod.Post,
			RequestUri = new Uri($"http://localhost:5053/api/parser-tasks/{createdTask.Id}/stop")
		};
		var stopTaskResponse = httpClient.Send(request);
		var stopTaskResponseContent = stopTaskResponse.Content.ReadAsStringAsync().Result;
		Assert.That(stopTaskResponse.IsSuccessStatusCode, Is.EqualTo(true));
		var stopTaskResponseDto = JsonConvert.DeserializeObject<StopParserTaskResponseDto>(stopTaskResponseContent)!;
		Assert.That(stopTaskResponseDto, Is.Not.EqualTo(null));
		
		// Проверяем что id остановленной задачи парсинга совпадает с созданной
		Assert.That(stopTaskResponseDto.ParserTaskId, Is.EqualTo(createdTask.Id));
	}
}