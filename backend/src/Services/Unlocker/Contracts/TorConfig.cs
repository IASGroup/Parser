namespace Unlocker.Contracts;

public class TorConfig
{
	public HttpClient? HttpClient { get; set; }
	public TorControl? TorControl { get; set; }
}