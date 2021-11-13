using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Slugent.APIInfo;

namespace Sample.APIInfo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			    .ConfigureServices((hostContext, services) => {
					// Set APIInfo Object and override the default root path to infotest...
					APIInfoBase apiInfoBase = new ("infotest");
					services.AddSingleton<APIInfoBase>(apiInfoBase);
			    })
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
	}
}
