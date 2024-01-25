namespace Collector.Tor;

public interface ITorIntegrationService
{
	public Task<string?> DownloadAsync(TorDownloadRequestDto torDownloadRequestDto);
	public Task<string?> ChangeIpAsync();
}