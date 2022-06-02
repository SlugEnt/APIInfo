using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SlugEnt.APIInfo;
using SlugEnt.APIInfo.HealthInfo;


namespace Sample.APIInfo
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample.APIInfo", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample.APIInfo v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				
				endpoints.MapControllers();
				endpoints.MapSlugEntPing();
				endpoints.MapSlugEntSimpleInfo();
				endpoints.MapSlugEntConfig();
				endpoints.MapSlugEntHealth(HealthEndPointConfig);
			});


			HealthCheckProcessor healthCheckProcessor = app.ApplicationServices.GetService<HealthCheckProcessor>();

			// Setup Health Check System
			//HealthCheckProcessor healthCheckProcessor = new HealthCheckProcessor((Microsoft.Extensions.Logging.ILogger)Log.Logger);
			ILogger<HealthCheckerFileSystem> hcfs = app.ApplicationServices.GetService<ILogger<HealthCheckerFileSystem>>();
			HealthCheckerFileSystem fileSystemA = new HealthCheckerFileSystem(hcfs,"Temp Folder", @"C:\temp", true, true);
			HealthCheckerFileSystem fileSystemB = new HealthCheckerFileSystem(hcfs,"Windows Folder", @"C:\windows", true, false);
			healthCheckProcessor.AddCheckItem(fileSystemA);
			healthCheckProcessor.AddCheckItem(fileSystemB);

		}


		public void HealthEndPointConfig(EndpointHealthConfig config)
		{
			config.Enabled = true;
		}

	}
}
