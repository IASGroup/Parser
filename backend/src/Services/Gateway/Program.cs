using Ocelot.DependencyInjection;
using Ocelot.Middleware;

new WebHostBuilder()
	.UseKestrel()
	.UseContentRoot(Directory.GetCurrentDirectory())
	.ConfigureAppConfiguration((hostingContext, config) =>
	{
		config
			.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
			.AddJsonFile("appsettings.json", true, true)
			.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
			.AddJsonFile("ocelot.json")
			.AddEnvironmentVariables();
	})
	.ConfigureServices((builder, s) =>
	{
		s.AddMvcCore();
		s.AddEndpointsApiExplorer();
		s.AddSwaggerForOcelot(builder.Configuration);
		s.AddOcelot();
		s.AddSignalR();
		s.AddCors(options =>
		{
			options.AddDefaultPolicy(builder =>
			{
				builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
			});
		});
	})
	// .ConfigureLogging((hostingContext, logging) =>
	// {
	// 
	// })
	.UseIISIntegration()
	.Configure(app =>
	{
		app.UseSwaggerForOcelotUI(opt =>
		{
			opt.PathToSwaggerGenerator = "/swagger/docs";
		});
		app.UseCors();
		app.UseWebSockets();
		app.UseOcelot().Wait();
	})
	.Build()
	.Run();