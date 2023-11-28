using System.Net;
using Unlocker.Contracts;

namespace Unlocker.Services;

public class TorConfigService : ITorConfigService
{
	private HttpClient? _httpClient;
	private TorControl? _torControl;

	public TorConfig ResetConfig(SetupTorConnectionDto setupDto)
	{
		_httpClient = new HttpClient(new HttpClientHandler
		{
			Proxy = new WebProxy($"socks5://localhost:{setupDto.SocksPort}")
		})
		{
			Timeout = TimeSpan.FromSeconds(10)
		};
		_torControl = new TorControl("localhost", setupDto.ControlPort, setupDto.ControlPassword);
		return new TorConfig { HttpClient = _httpClient, TorControl = _torControl };
	}

	public TorConfig GetConfig()
	{
		return new TorConfig { HttpClient = _httpClient, TorControl = _torControl };
	}

	public bool IsTorConfigured() => _httpClient is not null && _torControl is not null;
}