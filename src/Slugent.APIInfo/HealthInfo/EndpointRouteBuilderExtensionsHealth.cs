using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SlugEnt.ResourceHealthChecker;

namespace SlugEnt.APIInfo.HealthInfo
{
	public static partial class EndpointRouteBuilderExtensionsHealth
	{

		/// <summary>
		/// Adds a configuration endpoint to the <see cref="IEndpointRouteBuilder"/> for the Health info.
		/// </summary>
		/// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add endpoint to.</param>
		/// <param name="optionsDelegate">The EndpointHealthConfig object</param>
		/// <returns>A route for the endpoint.</returns>
		public static IEndpointConventionBuilder? MapSlugEntHealth(
			this IEndpointRouteBuilder endpoints)
				
			//Action<EndpointHealthConfig>? optionsDelegate = default)
		{
			if (endpoints == null) throw new ArgumentNullException(nameof(endpoints));

			HealthCheckProcessor healthCheckProcessor = endpoints.ServiceProvider.GetRequiredService<HealthCheckProcessor>();
			IAPIInfoBase apiInfoBase = endpoints.ServiceProvider.GetRequiredService<IAPIInfoBase>();
			string urlPattern = apiInfoBase.InfoRootPath + "/health";

//			var options = new EndpointHealthConfig();
//			optionsDelegate?.Invoke(options);

			//return MapHealthCore(endpoints, urlPattern, options);
			return MapHealthCore(endpoints, urlPattern);
		}


		/// <summary>
		/// Internal Health Endpoint configuration
		/// </summary>
		/// <param name="endpoints">The Endpoint object</param>
		/// <param name="pattern">The URL Pattern the endpoint is at</param>
		/// <param name="options">The options for the endpoint</param>
		/// <returns></returns>
		private static IEndpointConventionBuilder? MapHealthCore(
			IEndpointRouteBuilder endpoints,
			string pattern )//, EndpointHealthConfig options)
		{
			var builder = endpoints.CreateApplicationBuilder();

/*			if (!options.Enabled)
			{
				return null;
			}
*/
			var pipeline = builder
			               .UseMiddleware<EndpointHealthMiddleware>()
			               .Build();

			return endpoints.Map(pattern, pipeline);
		}
    }
}
