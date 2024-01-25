using System.Text;
using System.Text.Json;
using Collector.Contracts;
using Collector.Options;
using Microsoft.Extensions.Options;

namespace Collector.Tor;

public class TorIntegrationService: ITorIntegrationService
{
	private readonly IOptionsSnapshot<TorOptions> torOptions;
	private readonly HttpClient httpClient;
	
	public TorIntegrationService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<TorOptions> torOptions)
	{
		this.torOptions = torOptions;
		httpClient = httpClientFactory.CreateClient(HttpClientNames.Tor);
	}
	
	public async Task<string?> DownloadAsync(TorDownloadRequestDto torDownloadRequestDto)
	{
		var request = new HttpRequestMessage()
		{
			Content = new StringContent(JsonSerializer.Serialize(torDownloadRequestDto), Encoding.UTF8, "application/json"),
			Method = HttpMethod.Post,
			RequestUri = new Uri(torOptions.Value.BaseUrl + "/api/tor/download"),
		};
		var response = await httpClient.SendAsync(request);
		var responseContent = await response.Content.ReadAsStringAsync();
		return response.IsSuccessStatusCode ? responseContent : null;
	}

	public async Task<string?> ChangeIpAsync()
	{
		var requestUrl = torOptions.Value.BaseUrl + "/api/tor/changeip";
		var response = await httpClient.PostAsync(requestUrl, null);
		var responseContent = await response.Content.ReadAsStringAsync();
		return response.IsSuccessStatusCode ? responseContent : null;
	}
}