using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SlugEnt.APIInfo;

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
					apiInfoBase.AddConfigMatchCriteria("password",false,true);
					apiInfoBase.AddConfigMatchCriteria("os", false, true);
					apiInfoBase.AddConfigMatchCriteria("LogLevel", true, true);
					apiInfoBase.AddConfigMatchCriteria("environment", false, false);

					services.AddSingleton<IAPIInfoBase>(apiInfoBase);

					// Add a SimpleInfo retriever - Host Information
					services.AddTransient<ISimpleInfoRetriever, SimpleRetrieverHostInfo>();
					
			    })
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });
	}
}
