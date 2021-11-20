
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SlugEnt.APIInfo
{
	/// <summary>
    /// Adds extensions for the /Config route
    /// </summary>
    public static partial class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Adds the /ping endpoint to the <see cref="IEndpointRouteBuilder"/> for the ping info.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add endpoint to.</param>
        /// <param name="optionsDelegate">The EndpointPingConfig object</param>
        /// <returns>A route for the endpoint.</returns>
        public static IEndpointConventionBuilder? MapSlugEntConfig(
            this IEndpointRouteBuilder endpoints,
            //string pattern =  "info/ping",
            Action<EndpointConfigConfig>? optionsDelegate = default)
        {
            if (endpoints == null) throw new ArgumentNullException(nameof(endpoints));

            IAPIInfoBase apiInfoBase = endpoints.ServiceProvider.GetRequiredService<IAPIInfoBase>();
            string urlPattern = apiInfoBase.InfoRootPath + "/config";

            var options = new EndpointConfigConfig();
            optionsDelegate?.Invoke(options);

            return MapConfigCore(endpoints, urlPattern, options);
        }


        /// <summary>
        /// Internal Ping Endpoint configuration
        /// </summary>
        /// <param name="endpoints">The Endpoint object</param>
        /// <param name="pattern">The URL Pattern the endpoint is at</param>
        /// <param name="options">The options for the endpoint</param>
        /// <returns></returns>
        private static IEndpointConventionBuilder? MapConfigCore(
            IEndpointRouteBuilder endpoints,
            string pattern, EndpointConfigConfig options)
        {
            
            var builder = endpoints.CreateApplicationBuilder();

            if (!options.Enabled)
            {
                return null;
            }
            var pipeline = builder
                .UseMiddleware<EndpointConfigMiddleware>()
                .Build();

            return endpoints.Map(pattern, pipeline);
        }
    }
}