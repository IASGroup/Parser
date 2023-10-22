using Collector.Contexts;
using Collector.Options;
using Collector.ParserTasks;
using Collector.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(x => x.AddConsole());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.Name));
builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.Name));
builder.Services.AddHostedService<ConsumerNewParserTaskBackgroundService>();
builder.Services.AddSingleton<IParserTaskService, ParserTaskService>();
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();