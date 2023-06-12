using Autofac;
using Autofac.Extensions.DependencyInjection;
using NLog.Web;
using VocaDb.Web;

// Code from: https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6#3-update-programcs.
var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
logger.Debug("init main");

try
{
	// Code from: https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0&tabs=visual-studio#use-startup-with-the-new-minimal-hosting-model.

	var builder = WebApplication.CreateBuilder(args);
	builder.Configuration.AddJsonFile("appsettings.json", false, true);

	var startup = new Startup(builder.Configuration);

	startup.ConfigureServices(builder.Services);

	builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
	builder.Host.ConfigureContainer<ContainerBuilder>(startup.ConfigureContainer);

	// Code from: https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6#3-update-programcs.
	builder.Logging.ClearProviders();
	builder.Logging.SetMinimumLevel(LogLevel.Trace);
	builder.Host.UseNLog(new NLogAspNetCoreOptions
	{
		// See: https://nlog-project.org/2021/08/25/nlog-5-0-preview1-ready.html#nlogextensionslogging-without-any-filter.
		RemoveLoggerFactoryFilter = false,
	});

	var app = builder.Build();

	startup.Configure(app, app.Environment, app.Lifetime);

	app.Run();
}
catch (Exception ex)
{
	logger.Error(ex, "Stopped program because of exception");
	throw;
}
finally
{
	NLog.LogManager.Shutdown();
}
