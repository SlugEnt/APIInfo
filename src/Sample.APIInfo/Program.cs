using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
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
					apiInfoBase.AddConfigMatchCriteria("password");
					apiInfoBase.AddConfigMatchCriteria("os");
					apiInfoBase.AddConfigMatchCriteria("LogLevel", true);
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
