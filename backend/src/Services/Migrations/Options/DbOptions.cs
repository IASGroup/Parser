namespace Migrations.Options;

public class DbOptions
{
	public static string Name = nameof(DbOptions);
	public string Host { get; set; } = null!;
	public string DbName { get; set; } = null!;
	public string Port { get; set; } = null!;
	public string UserName { get; set; } = null!;
	public string UserPassword { get; set; } = null!;
}