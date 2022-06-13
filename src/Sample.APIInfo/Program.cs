using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using SlugEnt.APIInfo;
using SlugEnt.APIInfo.HealthInfo;
using SlugEnt.ResourceHealthChecker;


namespace Sample.APIInfo
{
	public class Program
	{

		public static void Main(string[] args) {
			Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
			                                      .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			                                      .Enrich.FromLogContext()
			                                      .WriteTo.Console()
			                                      .CreateLogger();
			Log.Information("Starting Host");


			CreateHostBuilder(args).Build().Run();
			
		}


		public static IHostBuilder CreateHostBuilder (string [] args) =>
			Host.CreateDefaultBuilder(args)
			    .UseSerilog()
			    .ConfigureServices((hostContext, services) => {



				    // Set APIInfo Object and override the default root path to infotest...
				    APIInfoBase apiInfoBase = new("infotest");
				    apiInfoBase.AddConfigHideCriteria("password");
				    apiInfoBase.AddConfigHideCriteria("os");
				    apiInfoBase.AddConfigHideCriteria("urls", false, false);
				    apiInfoBase.AddConfigHideCriteria("LogLevel", true);
				    apiInfoBase.AddConfigHideCriteria("environment", false, false);

				    // Override and allow one URLS entry to display
				    apiInfoBase.AddConfigOverrideString("ASPNETCORE_URLS");

				    services.AddSingleton<IAPIInfoBase>(apiInfoBase);


				    // Add a SimpleInfo retriever - Host Information
				    services.AddTransient<ISimpleInfoRetriever, SimpleRetrieverHostInfo>();


				    // Start the Health Checks
				    services.AddSingleton<HealthCheckProcessor>();

				    services.AddHostedService<APIBackgroundProcessor>();
			    })
			    .ConfigureWebHostDefaults(webBuilder => {
				    webBuilder.UseStartup<Startup>();
			    });
	}
}
