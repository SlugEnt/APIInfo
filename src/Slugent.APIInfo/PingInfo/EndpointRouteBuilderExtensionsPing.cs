using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SlugEnt.APIInfo;

namespace SlugEnt.APIInfo
{

    /// <summary>
    /// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add routes.
    /// </summary>
    public static partial class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Adds a configuration endpoint to the <see cref="IEndpointRouteBuilder"/> for the ping info.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add endpoint to.</param>
        /// <param name="pattern">The URL pattern of the endpoint.</param>
        /// <param name="optionsDelegate">The EndpointPingConfig object</param>
        /// <returns>A route for the endpoint.</returns>
        public static IEndpointConventionBuilder? MapSlugEntPing(
            this IEndpointRouteBuilder endpoints,
            //string pattern =  "info/ping",
            Action<EndpointPingConfig>? optionsDelegate = default)
        {
            if (endpoints == null) throw new ArgumentNullException(nameof(endpoints));
            
            IAPIInfoBase apiInfoBase = endpoints.ServiceProvider.GetRequiredService<IAPIInfoBase>();
            string urlPattern = apiInfoBase.InfoRootPath + "/ping";

            var options = new EndpointPingConfig();
            optionsDelegate?.Invoke(options);

            return MapPingCore(endpoints, urlPattern, options);
        }


        /// <summary>
        /// Internal Ping Endpoint configuration
        /// </summary>
        /// <param name="endpoints">The Endpoint object</param>
        /// <param name="pattern">The URL Pattern the endpoint is at</param>
        /// <param name="options">The options for the endpoint</param>
        /// <returns></returns>
        private static IEndpointConventionBuilder? MapPingCore(
            IEndpointRouteBuilder endpoints,
            string pattern, EndpointPingConfig options)
        {
            var environment = endpoints.ServiceProvider.GetRequiredService<IHostEnvironment>();
            var builder = endpoints.CreateApplicationBuilder();

            if (!options.Enabled )
            {
                return null;
            }
            var pipeline = builder
                .UseMiddleware<EndpointPingMiddleware>()
                .Build();

            return endpoints.Map(pattern, pipeline);
        }
    }
}