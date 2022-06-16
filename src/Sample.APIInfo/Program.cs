using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using ResourceHealthChecker.SqlServer;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using SlugEnt.APIInfo;
using SlugEnt.APIInfo.HealthInfo;
using SlugEnt.ResourceHealthChecker;
using SlugEnt.ResourceHealthChecker.SqlServer;


namespace SlugEnt.APIInfo.Sample
{
	/// <summary>
	/// Main Startup Application
	/// </summary>
	public class Program {
		private static WebApplication        _app;
		private static WebApplicationBuilder builder;
		private static HealthCheckProcessor  healthCheckProcessor;
		private static Serilog.ILogger       Logger;
		private static IConfiguration        _configuration;



		public static async Task Main (string [] args) {
			try {
				// Startup Logging immediately - even before we try to construct the host.
				Log.Logger = new LoggerConfiguration()
#if DEBUG
				             .MinimumLevel.Debug()
				             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
#else
						 .MinimumLevel.Information()
			             .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
#endif
				             .Enrich.FromLogContext()
				             .WriteTo.Console()
				             .CreateLogger();

				Log.Information("Starting " + Assembly.GetEntryAssembly().FullName);

				builder = WebApplication.CreateBuilder(args);
				Logger = Log.Logger;
				_configuration = builder.Configuration;
				string y = builder.Configuration ["Logging:LogLevel:Default"];
				string yy = builder.Configuration ["ConnectionStrings:AdventureDB"];
				await Setup(args);


				// Start the app!
				_app.Run();
			}
			catch ( Exception ex ) {
				Logger.Error("Unhandled Exception in the application resulted in application crash.  Exception was: " + ex.ToString());
			}
		}



		/// <summary>
		/// Main API Setup Method
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static async Task Setup(string[] args) {
			// 1.0 Initial Setup 


			// Initial Settings
			string corsPolicy = "CustomCors";

			// Get Sensitive Appsettings.json file location
			string sensitiveAppSettings = Environment.GetEnvironmentVariable("AppSettingSensitiveFolder");
			


			// 1.B.  We keep the AppSettings file in the root App folder on the servers so it never gets overwritten
			IWebHostEnvironment environment = builder.Environment;
			string versionPath = Directory.GetCurrentDirectory();
			DirectoryInfo appRootDirectoryInfo = Directory.GetParent(versionPath);
			string appRoot = appRootDirectoryInfo.FullName;
			Console.WriteLine("Running from Directory:  " + appRoot);
			
			// Load Environment Specific App Setting file
			string appSettingFileName = $"appsettings." + environment.EnvironmentName + ".json";
			string appSettingFile = Path.Join(appRoot, appSettingFileName);
			builder.Configuration.AddJsonFile(appSettingFile, true);
			DisplayAppSettingStatus(appSettingFile);
			

			// Load the Sensitive AppSettings.JSON file.
			string sensitiveFileName = Assembly.GetExecutingAssembly().GetName().Name + "_AppSettingsSensitive.json";
			appSettingFile = Path.Join(sensitiveAppSettings, sensitiveFileName);
			builder.Configuration.AddJsonFile(appSettingFile, true);
			DisplayAppSettingStatus(appSettingFile);


			// 1.C.  Logging in the App
			builder.Logging.ClearProviders();
			builder.Logging.AddSerilog(Log.Logger);
			builder.Host.UseSerilog(Log.Logger);


			// 1.D. Print the configuration
			// There are better ways to do this...
			var config = builder.Configuration.GetDebugView();
			System.Console.WriteLine(config);


			// 1.E.  Authentication and Authorization


			// 1.F.  Database Access


			// 1.G.  


			// 2.0 Services

			// 2.A - Main API App
			builder.Services.AddHostedService<APIBackgroundProcessor>();


			// 2.B - Setup APIInfo and Health Checks
			APIInfoBase apiInfoBase = SetupAPIInfoBase();
			builder.Services.AddSingleton<IAPIInfoBase>(apiInfoBase);

			// Add a SimpleInfo retriever - Host Information
			builder.Services.AddTransient<ISimpleInfoRetriever, SimpleRetrieverHostInfo>();

			// Start the Health Checks
			builder.Services.AddSingleton<HealthCheckProcessor>();


			// 2.Z


			// 3.0 - API Startup

			// 3.A - Detailed Error Handling
			builder.Services.AddProblemDetails(options => {
				       options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
				       options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
				       options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
			       })
			       .AddProblemDetailsConventions();

			// 3.B - Kestrel
			
			builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));
			IConfigurationSection allowedOriginsSection = builder.Configuration.GetSection("AllowedOrigins");
			string allowedOrigins = allowedOriginsSection.Value;
			string[] origins = allowedOrigins != null ? allowedOrigins.Split(";") : "".Split();
			

			// 3.C - CORS
			builder.Services.AddCors(opt => { opt.AddPolicy(corsPolicy, builder => { builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader(); }); });


			// 3.D - Swagger
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample.APIInfo", Version = "v1" });
			});

			// 3.E - Controllers
			builder.Services.AddControllers();


			// 4.0 - App Build and final Setup
			_app = builder.Build();
			_app.UseSerilogRequestLogging();

			// 4.A - If there is any additional Setup required by the API or instantation of static classes or configuration it should go here.

			// 4.B - Setup the HTTP Request Pipeline
			if (_app.Environment.IsDevelopment())
			{
				_app.UseSwagger();
				_app.UseSwaggerUI();
				_app.UseProblemDetails();
				_app.UseDeveloperExceptionPage();
			}



			_app.UseHttpsRedirection();
			_app.UseCors(corsPolicy);
			_app.UseRouting();


			/* this API is not using security
			app.UseAuthentication();
			app.UseAuthorization();
			*/


			// 4.C Endpoints
			_app.UseEndpoints(endpoints =>
			{

				endpoints.MapControllers();
				endpoints.MapSlugEntPing();
				endpoints.MapSlugEntSimpleInfo();
				endpoints.MapSlugEntConfig();
				endpoints.MapSlugEntHealth();
			});


#if DEBUG
			//if ( app.Environment.IsDevelopment() )
			_app.MapControllers().WithMetadata(new AllowAnonymousAttribute());
#else
			_app.MapControllers();
			
#endif


			// 5. Final Customization logic
			ConfigHealthChecker();
			await healthCheckProcessor.Start();

			// Exit if the Health Check has failed on start;
			EnumHealthStatus healthCheckStatus = healthCheckProcessor.Status;
			if (healthCheckStatus != EnumHealthStatus.Healthy)
			{
				Log.Fatal("Initial Health Startup Status is: " + healthCheckStatus.ToString() + "  Application is being shut down.");
				return;
			}



			Log.Logger.Error("Sample Error.  This is not a real error");
		}


		/// <summary>
		/// Sets up the API Info Base object
		/// </summary>
		/// <returns></returns>
		private static APIInfoBase SetupAPIInfoBase () {
			// This is how you override the api endpoint to something other than info
			//APIInfoBase apiInfoBase = new("infotest");
			APIInfoBase apiInfoBase = new();
			apiInfoBase.AddConfigHideCriteria("password");
			apiInfoBase.AddConfigHideCriteria("os");
			apiInfoBase.AddConfigHideCriteria("urls", false, false);
			apiInfoBase.AddConfigHideCriteria("LogLevel", true);
			apiInfoBase.AddConfigHideCriteria("environment", false, false);

			// Override and allow one URLS entry to display
			apiInfoBase.AddConfigOverrideString("ASPNETCORE_URLS");

			return apiInfoBase;
		}



		/// <summary>
		/// Configures The Health Checker 
		/// </summary>
		private static void ConfigHealthChecker () {
			// Setup Health Check System
			healthCheckProcessor = _app.Services.GetService<HealthCheckProcessor>();
			ILogger<HealthCheckerFileSystem> hcfs = _app.Services.GetService<ILogger<HealthCheckerFileSystem>>();

			HealthCheckerConfigFileSystem config2 = new HealthCheckerConfigFileSystem()
			{
				CheckIsWriteble = true,
				CheckIsReadable = true,
				FolderPath = @"C:\temp\HCW",
			};


			// We have 2 file system Checks.  One system is a Read check.  The other is a read write check.
			HealthCheckerFileSystem fileSystemA = new HealthCheckerFileSystem(hcfs, "Temp Folder Read", config2);
			HealthCheckerFileSystem fileSystemB = new HealthCheckerFileSystem(hcfs, "Temp Folder Write", config2);
			healthCheckProcessor.AddCheckItem(fileSystemA);
			healthCheckProcessor.AddCheckItem(fileSystemB);



			// We also will have a SQL Server Checker
			// Note: We are using the default Read and Write Tables for the Health Checker.  You will need to add these to AdventureWorks manually.
			// The 2 tables are --> SlugEntHealthCheck
			string adventureWorksConnStr = _configuration ["ConnectionStrings:AdventureDB"];
			HealthCheckerConfigSQLServer sqlConfig = new(adventureWorksConnStr);
			ILogger<HealthCheckerSQLServer> hcSQL = _app.Services.GetService<ILogger<HealthCheckerSQLServer>>();
			HealthCheckerSQLServer sqlCheck = new(hcSQL, "Adventure Works 2020", sqlConfig);
			healthCheckProcessor.AddCheckItem(sqlCheck);

		}


		/// <summary>
		/// Logs whether a given AppSettings file was found to exist.
		/// </summary>
		/// <param name="appSettingFileName"></param>
		private static void DisplayAppSettingStatus (string appSettingFileName) {
			if (File.Exists(appSettingFileName))
				Logger.Information("AppSettings File was located.  {AppSettingsFile}", appSettingFileName);
			else 
				Logger.Warning("AppSettings File was not found.  {AppSettingsFile}", appSettingFileName);
		}
	}
}
