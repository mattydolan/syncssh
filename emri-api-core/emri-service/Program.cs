using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace emri_service
{
	public class Program
	{
		public static void Main(string[] args)
		{
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
			catch (Exception exception)
			{
				// Catch NLog setup errors
				logger.Error(exception, "Stopped program because of exception");
				throw;
			}
			finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>();
              })
              .ConfigureLogging(logging =>
              {
				  logging.ClearProviders();
				  logging.SetMinimumLevel(LogLevel.Trace);
              })
              .UseNLog();  // Setup NLog for Dependency injection
    }
}
