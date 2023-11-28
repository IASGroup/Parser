using Unlocker.Contracts;

namespace Unlocker.Services;

public interface ITorConfigService
{
	TorConfig ResetConfig(SetupTorConnectionDto setupDto);
	TorConfig GetConfig();
	bool IsTorConfigured();
}