﻿using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Unlocker.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            _torControl = new TorControl("localhost", 9050, "");
        }

        [HttpGet("GetTorUrl")]
        public async Task<IActionResult> GetTorContent(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return Content(content);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get URL content by Tor: {ex.Message}");
            }
        }

        [HttpGet("ChangeTorIpUrl")]
        public async Task<IActionResult> ChangeTorIP(string url)
        {
            try
            {
                await _torControl.SignalNewNymAsync();

                await Task.Delay(5000);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();

                return Ok(responseContent);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get URL content with new Tor IP: {ex.Message}");
            }
        }
    }
}