using Hubs.Hubs;
using Hubs.RabbitMq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder =>
	{
		builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
	});
});
builder.Services.AddHostedService<ConsumerParserTaskCollectMessagesBackgroundService>();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();
app.MapHub<ParseTasksHub>("/parser-tasks-hub");
app.UseCors();

app.Run();