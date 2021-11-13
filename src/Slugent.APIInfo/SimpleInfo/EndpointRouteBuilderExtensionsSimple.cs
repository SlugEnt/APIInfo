using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slugent.APIInfo;

namespace Slugent.APIInfo.SimpleInfo
{

    /// <summary>
    /// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add routes.
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Adds a configuration endpoint to the <see cref="IEndpointRouteBuilder"/> for the ping info.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add endpoint to.</param>
        /// <param name="pattern">The URL pattern of the endpoint.</param>
        /// <param name="optionsDelegate">The EndpointSimpleConfig object</param>
        /// <returns>A route for the endpoint.</returns>
        public static IEndpointConventionBuilder? MapSimpleInfo(
            this IEndpointRouteBuilder endpoints,
            Action<EndpointSimpleConfig>? optionsDelegate = default)
        {
            if (endpoints == null) throw new ArgumentNullException(nameof(endpoints));
            
            APIInfoBase apiInfoBase = endpoints.ServiceProvider.GetRequiredService<APIInfoBase>();
            string urlPattern = apiInfoBase.InfoRootPath + "/simple";

            var options = new EndpointSimpleConfig();
            optionsDelegate?.Invoke(options);

            return MapSimpleInfoCore(endpoints, urlPattern, options);
        }


        /// <summary>
        /// Internal Ping Endpoint configuration
        /// </summary>
        /// <param name="endpoints">The Endpoint object</param>
        /// <param name="pattern">The URL Pattern the endpoint is at</param>
        /// <param name="options">The options for the endpoint</param>
        /// <returns></returns>
        private static IEndpointConventionBuilder? MapSimpleInfoCore(
            IEndpointRouteBuilder endpoints,
            string pattern, EndpointSimpleConfig options)
        {
            var environment = endpoints.ServiceProvider.GetRequiredService<IHostEnvironment>();
            var builder = endpoints.CreateApplicationBuilder();

            if (!options.Enabled )
            {
                return null;
            }
            var pipeline = builder
                .UseMiddleware<EndpointSimpleMiddleware>()
                .Build();

            return endpoints.Map(pattern, pipeline);
        }
    }
}