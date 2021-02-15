#nullable disable

using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace VocaDb.Web
{
	public class Program
	{
		private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
			// Code from: https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html#asp-net-core-3-0-and-generic-hosting
			.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureWebHostDefaults(webHostBuilder =>
			{
				webHostBuilder
					.UseContentRoot(Directory.GetCurrentDirectory())
					.UseIISIntegration()
					.UseStartup<Startup>();
			})
			// Code from: https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-3
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				logging.SetMinimumLevel(LogLevel.Trace);
			})
			.UseNLog();

		public static void Main(string[] args)
		{
			var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
			try
			{
				logger.Debug("init main");
				CreateHostBuilder(args).Build().Run();
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
		}
	}
}
