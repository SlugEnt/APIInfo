using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SlugEnt.ResourceHealthChecker;

namespace SlugEnt.APIInfo.HealthInfo
{
	public class EndpointHealthMiddleware {
		private HealthCheckProcessor _healthCheckProcessor;


		/// <summary>
		/// Initializes a new instance of <see cref="EndpointHealthMiddleware"/>.
		/// </summary>
		/// <param name="next"></param>
		public EndpointHealthMiddleware (RequestDelegate next, IServiceProvider serviceProvider) {
			_healthCheckProcessor = serviceProvider.GetRequiredService<HealthCheckProcessor>();
		}

		/// <summary>
		/// Executes the middleware that provides configuration-debug-view page.
		/// </summary>
		/// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
		public async Task InvokeAsync(HttpContext httpContext)
		{
			// TODO this is where the code for health check goes
			StringBuilder sb =  _healthCheckProcessor.DisplayFull();
			await httpContext.Response.WriteAsync(sb.ToString());
		}
	}

}
