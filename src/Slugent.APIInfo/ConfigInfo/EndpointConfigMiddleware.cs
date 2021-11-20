using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SlugEnt.APIInfo
{
	/// <summary>
	/// Handler for the endpoint /config.
	/// </summary>
	public class EndpointConfigMiddleware {

		private readonly IConfigurationRoot _configRoot;
		private readonly IAPIInfoBase       _apiInfoBase;


		/// <summary>
		/// Initializes a new instance of <see cref="EndpointSimpleMiddleware"/>.
		/// </summary>
		/// <param name="next"></param>
		/// <param name="configuration">IConfiguration object of current config values</param>
		/// <param name="apiInfoBase">The APIInfoBase object that contains settings we need</param>
		public EndpointConfigMiddleware (RequestDelegate next, IConfiguration configuration, IAPIInfoBase apiInfoBase) {
			_configRoot = configuration as ConfigurationRoot;
			_apiInfoBase = apiInfoBase;
		}


		/// <summary>
		/// Executes the middleware that provides configuration-debug-view page.
		/// </summary>
		/// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
 		/// <param name="configuration">IConfiguration object of current config values</param>
		/// <param name="apiInfoBase">The APIInfoBase object that contains settings we need</param>
		public async Task InvokeAsync(HttpContext httpContext, IConfiguration configuration, IAPIInfoBase apiInfoBase) {
			ConfigurationParser parser = new(_configRoot,_apiInfoBase);
			string returnHtml = parser.DisplayConfig();
			await httpContext.Response.WriteAsync(returnHtml);
		}


	}
}
