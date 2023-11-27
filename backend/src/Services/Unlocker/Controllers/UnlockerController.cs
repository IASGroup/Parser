using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Unlocker.Contracts;

namespace Unlocker.Controllers
{
	[ApiController]
	[Route("api/tor")]
	public class UnlockerController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly TorControl _torControl;

		public UnlockerController()
		{
			var handler = new HttpClientHandler
			{
				Proxy = new WebProxy("socks5://localhost:9050")
			};

			_httpClient = new HttpClient(handler);
			_httpClient.Timeout = TimeSpan.FromSeconds(10);
			_torControl = new TorControl("localhost", 9050, "");
		}

		[HttpPost("download")]
		public async Task<IActionResult> GetTorContentAsync([FromBody] HttpRequestDto requestDto)
		{
			try
			{
				var request = new HttpRequestMessage()
				{
					RequestUri = new Uri(requestDto.BaseUrl),
					Method = requestDto.MethodName.ToUpper() switch
					{
						"GET" => HttpMethod.Get,
						"POST" => HttpMethod.Post,
						"PUT" => HttpMethod.Put,
						"DELETE" => HttpMethod.Delete,
						"PATCH" => HttpMethod.Patch,
						_ => throw new ArgumentException("Метод запроса не найден", nameof(requestDto.MethodName))
					},
					Content = requestDto.Body is null ? null : new StringContent(requestDto.Body)
				};
				foreach (var header in requestDto.Headers)
				{
					request.Headers.Add(header.Name, header.Value);
				}

				var response = await _httpClient.SendAsync(request);
				response.EnsureSuccessStatusCode();
				var content = await response.Content.ReadAsStringAsync();
				return Content(content);
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to get URL content by Tor: {ex.Message}");
			}
		}

		[HttpPost("changeip")]
		public async Task<IActionResult> ChangeTorIPAsync()
		{
			try
			{
				await _torControl.SignalNewNymAsync();

				await Task.Delay(5000);

				return Ok(GetTorIpAddress());
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to change Tor IP: {ex.Message}");
			}
		}

		private string GetTorIpAddress()
		{
			try
			{
				var response = _httpClient.GetAsync("https://check.torproject.org/").Result;
				var result = response.Content.ReadAsStringAsync().Result;
				var regex = new Regex(@"([0-9]?[0-9]?[0-9]\.[0-9]?[0-9]?[0-9]\.[0-9]?[0-9]?[0-9]\.[0-9]?[0-9]?[0-9])");
				return regex.Match(result).Value;
			}
			catch (Exception ex)
			{
				return $"Failed to get Tor IP address: {ex.Message}";
			}
		}
	}
}