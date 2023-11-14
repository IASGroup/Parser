﻿using Collector.Contexts;
using Collector.Contracts;
using Collector.Options;
using Collector.ParserTasks;
using Collector.ParserTasks.Handlers.Api;
using Collector.ParserTasks.Share;
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
builder.Services.AddSingleton<IParserTaskApiHandleService, ParserTaskApiHandler>();
builder.Services.AddSingleton<IParserTaskUtilService, ParserTaskUtilService>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddHttpClient(HttpClientNames.Collector);

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