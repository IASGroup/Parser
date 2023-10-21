using System.Reflection;
using TaskManager.Contexts;
using TaskManager.Options;
using TaskManager.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x => x.CustomSchemaIds(y => y.FullName));
builder.Services.AddLogging();


builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.Name));
builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.Name));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

DbInitializer.Initialize(app.Services.CreateScope().ServiceProvider.GetService<AppDbContext>()!);

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