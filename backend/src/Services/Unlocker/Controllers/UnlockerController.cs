using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Unlocker.Contracts;
using Unlocker.Services;

namespace Unlocker.Controllers
{
	[ApiController]
	[Route("api/tor")]
	public class UnlockerController : Controller
	{
		private readonly ITorConfigService _torConfigService;

		public UnlockerController(ITorConfigService torConfigService)
		{
			_torConfigService = torConfigService;
		}

		[HttpPost("setup")]
		public async Task<IActionResult> SetupTorConnectionAsync([FromBody] SetupTorConnectionDto request)
		{
			try
			{
				_ = _torConfigService.ResetConfig(request);
				return Ok(await GetTorIpAddressAsync());
			}
			catch (Exception e)
			{
				return BadRequest("Setup failure");
			}
		}

		[HttpPost("download")]
		public async Task<IActionResult> GetTorContentAsync([FromBody] HttpRequestDto requestDto)
		{
			try
			{
				if (!_torConfigService.IsTorConfigured())
				{
					return BadRequest($"Tor is not configured");
				}

				var config = _torConfigService.GetConfig();
				var request = new HttpRequestMessage()
				{
					RequestUri = new Uri(requestDto.Url),
					Method = requestDto.MethodName.ToUpper() switch
					{
						"GET" => HttpMethod.Get,
						"POST" => HttpMethod.Post,
						"PUT" => HttpMethod.Put,
						"DELETE" => HttpMethod.Delete,
						"PATCH" => HttpMethod.Patch,
						_ => throw new ArgumentException("MethodName not found", nameof(requestDto.MethodName))
					},
					Content = requestDto.Body is null ? null : new StringContent(requestDto.Body)
				};
				foreach (var header in requestDto.Headers)
				{
					request.Headers.Add(header.Name, header.Value);
				}

				var response = await config.HttpClient!.SendAsync(request);
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
				if (!_torConfigService.IsTorConfigured())
				{
					return BadRequest($"Tor is not configured");
				}
				var config = _torConfigService.GetConfig();
				await Task.Delay(1000);
				await config.TorControl!.SignalNewNymAsync();
				return Ok(await GetTorIpAddressAsync());
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to change Tor IP: {ex.Message}");
			}
		}

		private async Task<string> GetTorIpAddressAsync()
		{
			try
			{
				if (!_torConfigService.IsTorConfigured()) throw new ArgumentException("Tor is not configured");
				var config = _torConfigService.GetConfig();
				var response = await config.HttpClient!.GetAsync("https://check.torproject.org/");
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