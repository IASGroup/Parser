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
		app.UseWebSockets();
		app.UseOcelot().Wait();
	})
	.Build()
	.Run();