namespace Collector.Options;

public class TorOptions
{
	public const string Section = nameof(TorOptions);
	public string BaseUrl { get; set; } = null!;
}