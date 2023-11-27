using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

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
            _torControl = new TorControl("localhost", 9050, "");
        }

        [HttpGet("download")]
        public async Task<IActionResult> GetTorContentAsync([FromQuery] string url)
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

        [HttpGet("gettorip")]
        public string GetTorIpAddress()
        {
            try
            {
                var response = _httpClient.GetAsync("https://check.torproject.org/").Result;
                var result = response.Content.ReadAsStringAsync().Result;

                var regex = new Regex(@"([0-9]?[0-9]?[0-9]\.[0-9]?[0-9]?[0-9]\.[0-9]?[0-9]?[0-9]\.[0-9]?[0-9]?[0-9])");
                var match = regex.Match(result);
                string extractedIP;

                extractedIP = match.Value;

                return extractedIP;
            }
            catch (Exception ex)
            {
                return $"Failed to get Tor IP address: {ex.Message}";
            }
        }
    }
}
