namespace Unlocker.Contracts;

public class SetupTorConnectionDto
{
	public int SocksPort { get; set; }
	public int ControlPort { get; set; }
	public string ControlPassword { get; set; } = null!;
}