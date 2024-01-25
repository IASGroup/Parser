namespace Collector.Tor;

public class TorDownloadRequestDto
{
	public string Url { get; set; } = null!;
	public string MethodName { get; set; } = null!;
	public string? Body { get; set; }
	public IEnumerable<RequestHeader> Headers { get; set; } = Enumerable.Empty<RequestHeader>();
}

public class RequestHeader
{
	public string Name { get; set; } = null!;
	public string Value { get; set; } = null!;
}