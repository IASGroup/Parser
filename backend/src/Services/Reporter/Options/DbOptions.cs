namespace TaskManager.Options;

public class DbOptions
{
	public static string Name = nameof(DbOptions);
	public string Host { get; set; }
	public string DbName { get; set; }
	public string Port { get; set; }
	public string UserName { get; set; }
	public string UserPassword { get; set; }
}